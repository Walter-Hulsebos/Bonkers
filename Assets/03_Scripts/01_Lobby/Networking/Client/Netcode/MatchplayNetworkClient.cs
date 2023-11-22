using System;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchplayNetworkClient : IDisposable
{
    public event Action<ConnectStatus> OnLocalConnection;
    public event Action<ConnectStatus> OnLocalDisconnection;

    private const Int32          TimeoutDuration = 10;
    private       NetworkManager networkManager;
    private       RelayJoinData  relayJoinData;

    private DisconnectReason DisconnectReason { get; } = new ();

    private const String MenuSceneName = "Menu";

    public MatchplayNetworkClient()
    {
        networkManager                            =  NetworkManager.Singleton;
        networkManager.OnClientDisconnectCallback += RemoteDisconnect;
    }

    public void StartClient(String ip, Int32 port)
    {
        UnityTransport unityTransport = networkManager.gameObject.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ipv4Address: ip, port: (UInt16)port);
        ConnectClient();
    }

    public async Task<JoinAllocation> StartClient(String joinCode)
    {
        JoinAllocation allocation = null;

        try { allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode); }
        catch (Exception e)
        {
            Debug.Log(message: e);
            return null;
        }

        relayJoinData = new RelayJoinData
        {
            Key                = allocation.Key,
            Port               = (UInt16)allocation.RelayServer.Port,
            AllocationID       = allocation.AllocationId,
            AllocationIDBytes  = allocation.AllocationIdBytes,
            ConnectionData     = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            IPv4Address        = allocation.RelayServer.IpV4,
        };

        UnityTransport unityTransport = networkManager.gameObject.GetComponent<UnityTransport>();

        unityTransport.SetRelayServerData
        (
            ipv4Address: relayJoinData.IPv4Address,
            port: relayJoinData.Port,
            allocationIdBytes: relayJoinData.AllocationIDBytes,
            keyBytes: relayJoinData.Key,
            connectionDataBytes: relayJoinData.ConnectionData,
            hostConnectionDataBytes: relayJoinData.HostConnectionData
        );

        ConnectClient();

        return allocation;
    }

    public void DisconnectClient()
    {
        DisconnectReason.SetDisconnectReason(reason: ConnectStatus.UserRequestedDisconnect);
        NetworkShutdown();
    }

    private void ConnectClient()
    {
        UserData userData = ClientSingleton.Instance.Manager.User.Data;

        String payload      = JsonUtility.ToJson(obj: userData);
        Byte[] payloadBytes = Encoding.UTF8.GetBytes(s: payload);

        networkManager.NetworkConfig.ConnectionData                = payloadBytes;
        networkManager.NetworkConfig.ClientConnectionBufferTimeout = TimeoutDuration;

        if (networkManager.StartClient())
        {
            Debug.Log(message: "Starting Client!");

            RegisterListeners();
        }
        else
        {
            Debug.LogWarning(message: "Could not start Client!");
            OnLocalDisconnection?.Invoke(obj: ConnectStatus.Undefined);
        }
    }

    public void RegisterListeners()
    {
        MatchplayNetworkMessenger.RegisterListener(messageType: NetworkMessage.LocalClientConnected,    listenerMethod: ReceiveLocalClientConnectStatus);
        MatchplayNetworkMessenger.RegisterListener(messageType: NetworkMessage.LocalClientDisconnected, listenerMethod: ReceiveLocalClientDisconnectStatus);
    }

    private void ReceiveLocalClientConnectStatus(UInt64 clientId, FastBufferReader reader)
    {
        reader.ReadValueSafe(value: out ConnectStatus status);

        Debug.Log(message: "ReceiveLocalClientConnectStatus: " + status);

        if (status != ConnectStatus.Success) { DisconnectReason.SetDisconnectReason(reason: status); }

        OnLocalConnection?.Invoke(obj: status);
    }

    private void ReceiveLocalClientDisconnectStatus(UInt64 clientId, FastBufferReader reader)
    {
        reader.ReadValueSafe(value: out ConnectStatus status);
        Debug.Log(message: "ReceiveLocalClientDisconnectStatus: " + status);
        DisconnectReason.SetDisconnectReason(reason: status);
    }

    private void RemoteDisconnect(UInt64 clientId)
    {
        Debug.Log(message: $"Got Client Disconnect callback for {clientId}");

        if (clientId != 0 && clientId != networkManager.LocalClientId) { return; }

        NetworkShutdown();
    }

    private void NetworkShutdown()
    {
        // Take client back to the main menu if they aren't already there
        if (SceneManager.GetActiveScene().name != MenuSceneName) { SceneManager.LoadScene(sceneName: MenuSceneName); }

        // If we are already on the main menu then it means we timed-out
        if (networkManager.IsConnectedClient) { networkManager.Shutdown(); }

        OnLocalDisconnection?.Invoke(obj: DisconnectReason.Reason);
        MatchplayNetworkMessenger.UnRegisterListener(messageType: NetworkMessage.LocalClientConnected);
        MatchplayNetworkMessenger.UnRegisterListener(messageType: NetworkMessage.LocalClientDisconnected);
    }

    public void Dispose()
    {
        if (networkManager != null && networkManager.CustomMessagingManager != null) { networkManager.OnClientConnectedCallback -= RemoteDisconnect; }
    }
}

public struct RelayJoinData
{
    public String JoinCode;
    public String IPv4Address;
    public UInt16 Port;
    public Guid   AllocationID;
    public Byte[] AllocationIDBytes;
    public Byte[] ConnectionData;
    public Byte[] HostConnectionData;
    public Byte[] Key;
}