using System;

using Unity.Netcode;

public enum NetworkMessage
{
    LocalClientConnected,
    LocalClientDisconnected,
}

public class MatchplayNetworkMessenger
{
    public static void SendMessageToAll(NetworkMessage messageType, FastBufferWriter writer)
    {
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(messageType.ToString(), writer);
    }

    public static void SendMessageTo(NetworkMessage messageType, UInt64 clientId, FastBufferWriter writer)
    {
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(messageType.ToString(), clientId, writer);
    }

    public static void RegisterListener(NetworkMessage messageType, CustomMessagingManager.HandleNamedMessageDelegate listenerMethod)
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(messageType.ToString(), listenerMethod);
    }

    public static void UnRegisterListener(NetworkMessage messageType)
    {
        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(messageType.ToString());
    }
}