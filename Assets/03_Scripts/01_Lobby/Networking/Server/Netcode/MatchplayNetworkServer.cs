using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using UnityEngine;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

public class MatchplayNetworkServer : IDisposable
{
    public static MatchplayNetworkServer Instance { get; private set; }

    public Action<Matchplayer> OnServerPlayerAdded;
    public Action<Matchplayer> OnServerPlayerRemoved;

    public Action<UserData> OnPlayerJoined;
    public Action<UserData> OnPlayerLeft;

    private SynchedServerData synchedServerData;
    private NetworkManager    networkManager;

    public Action<String> OnClientLeft;

    private const Int32 MaxConnectionPayload = 1024;

    private Boolean gameHasStarted;

    public Dictionary<String, UserData> ClientData     { get; private set; } = new ();
    public Dictionary<UInt64, String>   ClientIdToAuth { get; private set; } = new ();

    public MatchplayNetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        this.networkManager.ConnectionApprovalCallback += ApprovalCheck;
        this.networkManager.OnServerStarted            += OnNetworkReady;

        Instance = this;
    }

    public Boolean OpenConnection(String ip, Int32 port, GameInfo startingGameInfo)
    {
        UnityTransport unityTransport = networkManager.gameObject.GetComponent<UnityTransport>();
        networkManager.NetworkConfig.NetworkTransport = unityTransport;
        unityTransport.SetConnectionData(ip, (UInt16)port);
        Debug.Log($"Starting server at {ip}:{port}\nWith: {startingGameInfo}");

        return networkManager.StartServer();
    }

    public async Task<SynchedServerData> ConfigureServer(GameInfo startingGameInfo)
    {
        networkManager.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);

        Boolean localNetworkedSceneLoaded = false;
        networkManager.SceneManager.OnLoadComplete += CreateAndSetSynchedServerData;

        void CreateAndSetSynchedServerData(UInt64 clientId, String sceneName, LoadSceneMode sceneMode)
        {
            if (clientId != networkManager.LocalClientId) { return; }

            localNetworkedSceneLoaded                  =  true;
            networkManager.SceneManager.OnLoadComplete -= CreateAndSetSynchedServerData;
        }

        Task waitTask = WaitUntilSceneLoaded();

        async Task WaitUntilSceneLoaded()
        {
            while (!localNetworkedSceneLoaded) { await Task.Delay(50); }
        }

        if (await Task.WhenAny(new[] { waitTask, Task.Delay(5000), }) != waitTask)
        {
            Debug.LogWarning($"Timed out waiting for Server Scene Loading: Not able to Load Scene");
            return null;
        }

        synchedServerData = Object.Instantiate(Resources.Load<SynchedServerData>("SynchedServerData"));
        synchedServerData.GetComponent<NetworkObject>().Spawn();

        synchedServerData.map.Value       = startingGameInfo.map;
        synchedServerData.gameMode.Value  = startingGameInfo.gameMode;
        synchedServerData.gameQueue.Value = startingGameInfo.gameQueue;

        Debug.Log
        (
            $"Synched Server Values: {synchedServerData.map.Value} - {synchedServerData.gameMode.Value} - {synchedServerData.gameQueue.Value}",
            synchedServerData.gameObject
        );

        return synchedServerData;
    }

    public void SetCharacter(UInt64 clientId, Int32 characterId)
    {
        if (ClientIdToAuth.TryGetValue(clientId, out String auth))
        {
            if (ClientData.TryGetValue(auth, out UserData data)) { data.characterId = characterId; }
        }
    }

    public void StartGame(string scene = "Arena 1")
    {
        gameHasStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single); //TODO: add map select
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (request.Payload.Length > MaxConnectionPayload || gameHasStarted)
        {
            response.Approved           = false;
            response.CreatePlayerObject = false;
            response.Position           = null;
            response.Rotation           = null;
            response.Pending            = false;

            return;
        }

        String   payload  = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);
        userData.clientId = request.ClientNetworkId;
        Debug.Log($"Host ApprovalCheck: connecting client: ({request.ClientNetworkId}) - {userData}");

        if (ClientData.ContainsKey(userData.userAuthId))
        {
            UInt64 oldClientId = ClientData[userData.userAuthId].clientId;
            Debug.Log($"Duplicate ID Found : {userData.userAuthId}, Disconnecting Old user");

            SendClientDisconnected(request.ClientNetworkId, ConnectStatus.LoggedInAgain);
            WaitToDisconnect(oldClientId);
        }

        SendClientConnected(request.ClientNetworkId, ConnectStatus.Success);

        ClientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        ClientData[userData.userAuthId]         = userData;
        OnPlayerJoined?.Invoke(userData);

        response.Approved           = true;
        response.CreatePlayerObject = true;
        response.Rotation           = Quaternion.identity;
        response.Pending            = false;

        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

        Task.Factory.StartNew
        (
            async () => await SetupPlayerPrefab(request.ClientNetworkId),
            System.Threading.CancellationToken.None,
            TaskCreationOptions.None,
            scheduler
        );
    }

    private void OnNetworkReady() { networkManager.OnClientDisconnectCallback += OnClientDisconnect; }

    private void OnClientDisconnect(UInt64 clientId)
    {
        SendClientDisconnected(clientId, ConnectStatus.GenericDisconnect);

        if (ClientIdToAuth.TryGetValue(clientId, out String authId))
        {
            ClientIdToAuth?.Remove(clientId);
            OnPlayerLeft?.Invoke(ClientData[authId]);

            if (ClientData[authId].clientId == clientId)
            {
                ClientData.Remove(authId);
                OnClientLeft?.Invoke(authId);
            }
        }

        Matchplayer matchPlayerInstance = GetNetworkedMatchPlayer(clientId);
        OnServerPlayerRemoved?.Invoke(matchPlayerInstance);
    }

    private void SendClientConnected(UInt64 clientId, ConnectStatus status)
    {
        FastBufferWriter writer = new (sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        Debug.Log($"Send Network Client Connected to : {clientId}");
        MatchplayNetworkMessenger.SendMessageTo(NetworkMessage.LocalClientConnected, clientId, writer);
    }

    private void SendClientDisconnected(UInt64 clientId, ConnectStatus status)
    {
        FastBufferWriter writer = new (sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        Debug.Log($"Send networkClient Disconnected to : {clientId}");
        MatchplayNetworkMessenger.SendMessageTo(NetworkMessage.LocalClientDisconnected, clientId, writer);
    }

    private async void WaitToDisconnect(UInt64 clientId)
    {
        await Task.Delay(500);
        networkManager.DisconnectClient(clientId);
    }

    private async Task SetupPlayerPrefab(UInt64 clientId)
    {
        NetworkObject playerNetworkObject;

        do
        {
            playerNetworkObject = networkManager.SpawnManager.GetPlayerNetworkObject(clientId);
            await Task.Delay(100);
        }
        while (playerNetworkObject == null);

        OnServerPlayerAdded?.Invoke(GetNetworkedMatchPlayer(clientId));
    }

    public UserData GetUserDataByClientId(UInt64 clientId)
    {
        if (ClientIdToAuth.TryGetValue(clientId, out String authId))
        {
            if (ClientData.TryGetValue(authId, out UserData data)) { return data; }

            return null;
        }

        return null;
    }

    private Matchplayer GetNetworkedMatchPlayer(UInt64 clientId)
    {
        NetworkObject playerObject = networkManager.SpawnManager.GetPlayerNetworkObject(clientId);
        return playerObject.GetComponent<Matchplayer>();
    }

    public void Dispose()
    {
        if (networkManager == null) { return; }

        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        networkManager.OnServerStarted            -= OnNetworkReady;

        if (networkManager.IsListening) { networkManager.Shutdown(); }
    }
}