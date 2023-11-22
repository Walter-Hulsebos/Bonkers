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
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(messageName: messageType.ToString(), messageStream: writer);
    }

    public static void SendMessageTo(NetworkMessage messageType, UInt64 clientId, FastBufferWriter writer)
    {
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(messageName: messageType.ToString(), clientId: clientId, messageStream: writer);
    }

    public static void RegisterListener(NetworkMessage messageType, CustomMessagingManager.HandleNamedMessageDelegate listenerMethod)
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(name: messageType.ToString(), callback: listenerMethod);
    }

    public static void UnRegisterListener(NetworkMessage messageType)
    {
        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(name: messageType.ToString());
    }
}