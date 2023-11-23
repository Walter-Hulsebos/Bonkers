using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    [SerializeField] private Int32 maxConnections = 4;

    private static HostSingleton hostSingleton;

    public static HostSingleton Instance
    {
        get
        {
            if (hostSingleton != null) { return hostSingleton; }

            hostSingleton = FindObjectOfType<HostSingleton>();

            if (hostSingleton == null)
            {
                Debug.LogError(message: "No HostSingleton in scene, did you run this from the bootStrap scene?");
                return null;
            }

            return hostSingleton;
        }
    }

    public  MatchplayNetworkServer NetworkServer { get; private set; }
    public  RelayHostData          RelayHostData => relayHostData;
    private RelayHostData          relayHostData;
    private String                 lobbyId;

    private void Start() { DontDestroyOnLoad(target: gameObject); }

    private void OnDestroy() { Shutdown(); }

    public async Task<Boolean> StartHostAsync()
    {
        Allocation allocation = null;

        try
        {
            //Ask Unity Services to allocate a Relay server
            allocation = await Relay.Instance.CreateAllocationAsync(maxConnections: maxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(message: e);
            return false;
        }

        //Populate the hosting data
        relayHostData = new RelayHostData
        {
            Key               = allocation.Key,
            Port              = (UInt16)allocation.RelayServer.Port,
            AllocationID      = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData    = allocation.ConnectionData,
            IPv4Address       = allocation.RelayServer.IpV4,
        };

        try
        {
            //Retrieve the Relay join code for our clients to join our party
            relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(allocationId: RelayHostData.AllocationID);

            Debug.Log(message: RelayHostData.JoinCode);
        }
        catch (Exception e)
        {
            Debug.Log(message: e);
            return false;
        }

        //Retrieve the Unity transport used by the NetworkManager
        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

        transport.SetRelayServerData
            (ipv4Address: RelayHostData.IPv4Address, port: RelayHostData.Port, allocationIdBytes: RelayHostData.AllocationIDBytes, keyBytes: RelayHostData.Key, connectionDataBytes: RelayHostData.ConnectionData);

        try
        {
            CreateLobbyOptions createLobbyOptions = new ();
            createLobbyOptions.IsPrivate = false;

            createLobbyOptions.Data = new Dictionary<String, DataObject>()
            {
                { "JoinCode", new DataObject(visibility: DataObject.VisibilityOptions.Member, value: RelayHostData.JoinCode) },
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName: "My Lobby", maxPlayers: maxConnections, options: createLobbyOptions);
            lobbyId = lobby.Id;
            StartCoroutine(routine: HeartbeatLobbyCoroutine(waitTimeSeconds: 15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(message: e);
            return false;
        }

        UserData userData = ClientSingleton.Instance.Manager.User.Data;

        String payload      = JsonUtility.ToJson(obj: userData);
        Byte[] payloadBytes = Encoding.UTF8.GetBytes(s: payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkServer = new MatchplayNetworkServer(networkManager: NetworkManager.Singleton);

        NetworkManager.Singleton.StartHost();

        #pragma warning disable 4014
        await NetworkServer.ConfigureServer(startingGameInfo: new GameInfo { map = Map.Default, });
        #pragma warning restore 4014

        ClientSingleton.Instance.Manager.NetworkClient.RegisterListeners();

        NetworkServer.OnClientLeft += OnClientDisconnect;

        return true;
    }

    private IEnumerator HeartbeatLobbyCoroutine(Single waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new (time: waitTimeSeconds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId: lobbyId);
            yield return delay;
        }
    }

    private async void OnClientDisconnect(String authId)
    {
        try { await LobbyService.Instance.RemovePlayerAsync(lobbyId: lobbyId, playerId: authId); }
        catch (LobbyServiceException e) { Debug.Log(message: e); }
    }

    public async void Shutdown()
    {
        StopCoroutine(methodName: nameof(HeartbeatLobbyCoroutine));

        if (String.IsNullOrEmpty(value: lobbyId)) { return; }

        try { await Lobbies.Instance.DeleteLobbyAsync(lobbyId: lobbyId); }
        catch (LobbyServiceException e) { Debug.Log(message: e); }

        lobbyId = String.Empty;

        NetworkServer.OnClientLeft -= OnClientDisconnect;

        NetworkServer?.Dispose();
    }
}

public struct RelayHostData
{
    public String JoinCode;
    public String IPv4Address;
    public UInt16 Port;
    public Guid   AllocationID;
    public Byte[] AllocationIDBytes;
    public Byte[] ConnectionData;
    public Byte[] Key;
}