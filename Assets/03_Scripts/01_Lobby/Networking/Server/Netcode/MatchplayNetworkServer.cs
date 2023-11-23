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
        unityTransport.SetConnectionData(ipv4Address: ip, port: (UInt16)port);
        Debug.Log(message: $"Starting server at {ip}:{port}\nWith: {startingGameInfo}");

        return networkManager.StartServer();
    }

    public async Task<SynchedServerData> ConfigureServer(GameInfo startingGameInfo)
    {
        networkManager.SceneManager.LoadScene(sceneName: "CharacterSelect", loadSceneMode: LoadSceneMode.Single);

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
            while (!localNetworkedSceneLoaded) { await Task.Delay(millisecondsDelay: 50); }
        }

        if (await Task.WhenAny(tasks: new[] { waitTask, Task.Delay(millisecondsDelay: 5000) }) != waitTask)
        {
            Debug.LogWarning(message: $"Timed out waiting for Server Scene Loading: Not able to Load Scene");
            return null;
        }

        synchedServerData = Object.Instantiate(original: Resources.Load<SynchedServerData>(path: "SynchedServerData"));
        synchedServerData.GetComponent<NetworkObject>().Spawn();

        synchedServerData.map.Value       = startingGameInfo.map;
        synchedServerData.gameMode.Value  = startingGameInfo.gameMode;
        synchedServerData.gameQueue.Value = startingGameInfo.gameQueue;

        Debug.Log
        (
            message: $"Synched Server Values: {synchedServerData.map.Value} - {synchedServerData.gameMode.Value} - {synchedServerData.gameQueue.Value}",
            context: synchedServerData.gameObject
        );

        return synchedServerData;
    }

    public void SetCharacter(UInt64 clientId, Int32 characterId)
    {
        if (ClientIdToAuth.TryGetValue(key: clientId, value: out String auth))
        {
            if (ClientData.TryGetValue(key: auth, value: out UserData data)) { data.characterId = characterId; }
        }
    }

    public void StartGame()
    {
        gameHasStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(sceneName: "T3_Final", loadSceneMode: LoadSceneMode.Single);
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

        String   payload  = System.Text.Encoding.UTF8.GetString(bytes: request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(json: payload);
        userData.clientId = request.ClientNetworkId;
        Debug.Log(message: $"Host ApprovalCheck: connecting client: ({request.ClientNetworkId}) - {userData}");

        if (ClientData.ContainsKey(key: userData.userAuthId))
        {
            UInt64 oldClientId = ClientData[key: userData.userAuthId].clientId;
            Debug.Log(message: $"Duplicate ID Found : {userData.userAuthId}, Disconnecting Old user");

            SendClientDisconnected(clientId: request.ClientNetworkId, status: ConnectStatus.LoggedInAgain);
            WaitToDisconnect(clientId: oldClientId);
        }

        SendClientConnected(clientId: request.ClientNetworkId, status: ConnectStatus.Success);

        ClientIdToAuth[key: request.ClientNetworkId] = userData.userAuthId;
        ClientData[key: userData.userAuthId]         = userData;
        OnPlayerJoined?.Invoke(obj: userData);

        response.Approved           = true;
        response.CreatePlayerObject = true;
        response.Rotation           = Quaternion.identity;
        response.Pending            = false;

        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

        Task.Factory.StartNew
        (
            function: async () => await SetupPlayerPrefab(clientId: request.ClientNetworkId),
            cancellationToken: System.Threading.CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: scheduler
        );
    }

    private void OnNetworkReady() { networkManager.OnClientDisconnectCallback += OnClientDisconnect; }

    private void OnClientDisconnect(UInt64 clientId)
    {
        SendClientDisconnected(clientId: clientId, status: ConnectStatus.GenericDisconnect);

        if (ClientIdToAuth.TryGetValue(key: clientId, value: out String authId))
        {
            ClientIdToAuth?.Remove(key: clientId);
            OnPlayerLeft?.Invoke(obj: ClientData[key: authId]);

            if (ClientData[key: authId].clientId == clientId)
            {
                ClientData.Remove(key: authId);
                OnClientLeft?.Invoke(obj: authId);
            }
        }

        Matchplayer matchPlayerInstance = GetNetworkedMatchPlayer(clientId: clientId);
        OnServerPlayerRemoved?.Invoke(obj: matchPlayerInstance);
    }

    private void SendClientConnected(UInt64 clientId, ConnectStatus status)
    {
        FastBufferWriter writer = new (size: sizeof(ConnectStatus), allocator: Allocator.Temp);
        writer.WriteValueSafe(value: status);
        Debug.Log(message: $"Send Network Client Connected to : {clientId}");
        MatchplayNetworkMessenger.SendMessageTo(messageType: NetworkMessage.LocalClientConnected, clientId: clientId, writer: writer);
    }

    private void SendClientDisconnected(UInt64 clientId, ConnectStatus status)
    {
        FastBufferWriter writer = new (size: sizeof(ConnectStatus), allocator: Allocator.Temp);
        writer.WriteValueSafe(value: status);
        Debug.Log(message: $"Send networkClient Disconnected to : {clientId}");
        MatchplayNetworkMessenger.SendMessageTo(messageType: NetworkMessage.LocalClientDisconnected, clientId: clientId, writer: writer);
    }

    private async void WaitToDisconnect(UInt64 clientId)
    {
        await Task.Delay(millisecondsDelay: 500);
        networkManager.DisconnectClient(clientId: clientId);
    }

    private async Task SetupPlayerPrefab(UInt64 clientId)
    {
        NetworkObject playerNetworkObject;

        do
        {
            playerNetworkObject = networkManager.SpawnManager.GetPlayerNetworkObject(clientId: clientId);
            await Task.Delay(millisecondsDelay: 100);
        }
        while (playerNetworkObject == null);

        OnServerPlayerAdded?.Invoke(obj: GetNetworkedMatchPlayer(clientId: clientId));
    }

    public UserData GetUserDataByClientId(UInt64 clientId)
    {
        if (ClientIdToAuth.TryGetValue(key: clientId, value: out String authId))
        {
            if (ClientData.TryGetValue(key: authId, value: out UserData data)) { return data; }

            return null;
        }

        return null;
    }

    private Matchplayer GetNetworkedMatchPlayer(UInt64 clientId)
    {
        NetworkObject playerObject = networkManager.SpawnManager.GetPlayerNetworkObject(clientId: clientId);
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