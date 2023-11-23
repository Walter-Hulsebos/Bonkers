using System;

using Unity.Netcode;

public struct CharacterSelectState : INetworkSerializable,
                                     IEquatable<CharacterSelectState>
{
    public UInt64  ClientId;
    public Int32   CharacterId;
    public Boolean IsLockedIn;

    public CharacterSelectState(UInt64 clientId, Int32 characterId = -1, Boolean isLockedIn = false)
    {
        ClientId    = clientId;
        CharacterId = characterId;
        IsLockedIn  = isLockedIn;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref CharacterId);
        serializer.SerializeValue(ref IsLockedIn);
    }

    public Boolean Equals(CharacterSelectState other) =>
        ClientId == other.ClientId && CharacterId == other.CharacterId && IsLockedIn == other.IsLockedIn;
}