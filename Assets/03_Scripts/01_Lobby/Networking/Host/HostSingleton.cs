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
                Debug.LogError("No HostSingleton in scene, did you run this from the bootStrap scene?");
                return null;
            }

            return hostSingleton;
        }
    }

    public MatchplayNetworkServer NetworkServer { get; private set; }

    public RelayHostData HostRelayData
    {
        get => _hostRelayDataBackingField;
        private set => _hostRelayDataBackingField = value;
    }

    private RelayHostData _hostRelayDataBackingField;
    private String        lobbyId;

    private void Start() { DontDestroyOnLoad(gameObject); }

    private void OnDestroy() { Shutdown(); }

    public async Task<Boolean> StartHostAsync()
    {
        Allocation allocation = null;

        try
        {
            //Ask Unity Services to allocate a Relay server
            allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }

        //Populate the hosting data
        HostRelayData = new RelayHostData
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
            _hostRelayDataBackingField.JoinCode = await Relay.Instance.GetJoinCodeAsync(HostRelayData.AllocationID);

            Debug.Log(HostRelayData.JoinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }

        //Retrieve the Unity transport used by the NetworkManager
        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

        transport.SetRelayServerData
            (HostRelayData.IPv4Address, HostRelayData.Port, HostRelayData.AllocationIDBytes, HostRelayData.Key, HostRelayData.ConnectionData);

        try
        {
            CreateLobbyOptions createLobbyOptions = new ();
            createLobbyOptions.IsPrivate = false;

            createLobbyOptions.Data = new Dictionary<String, DataObject>()
            {
                { "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, HostRelayData.JoinCode) },
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("My Lobby", maxConnections, createLobbyOptions);
            lobbyId = lobby.Id;
            StartCoroutine(HeartbeatLobbyCoroutine(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }

        UserData userData = ClientSingleton.Instance.Manager.User.Data;

        String payload      = JsonUtility.ToJson(userData);
        Byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkServer = new MatchplayNetworkServer(NetworkManager.Singleton);

        NetworkManager.Singleton.StartHost();

        #pragma warning disable 4014
        await NetworkServer.ConfigureServer(new GameInfo { map = Map.Default, });
        #pragma warning restore 4014

        ClientSingleton.Instance.Manager.NetworkClient.RegisterListeners();

        NetworkServer.OnClientLeft += OnClientDisconnect;

        return true;
    }

    private IEnumerator HeartbeatLobbyCoroutine(Single waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new (waitTimeSeconds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private async void OnClientDisconnect(String authId)
    {
        try { await LobbyService.Instance.RemovePlayerAsync(lobbyId, authId); }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }

    public async void Shutdown()
    {
        StopCoroutine(nameof(HeartbeatLobbyCoroutine));

        if (String.IsNullOrEmpty(lobbyId)) { return; }

        try { await Lobbies.Instance.DeleteLobbyAsync(lobbyId); }
        catch (LobbyServiceException e) { Debug.Log(e); }

        lobbyId = String.Empty;

        NetworkServer.OnClientLeft -= OnClientDisconnect;

        NetworkServer?.Dispose();
    }
}

public struct RelayHostData
{
    public String JoinCode;

    // ReSharper disable once InconsistentNaming
    public String IPv4Address;
    public UInt16 Port;
    public Guid   AllocationID;
    public Byte[] AllocationIDBytes;
    public Byte[] ConnectionData;
    public Byte[] Key;
}