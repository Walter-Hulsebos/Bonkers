using System;

using Bonkers.Shared;

using Unity.Netcode;

using U08 = System.Byte;
using U64 = System.UInt64;
using I32 = System.Int32;

public struct CharacterSelectState : INetworkSerializable,
                                     IEquatable<CharacterSelectState>
{
    public U64     ClientId;
    public I32     TeamId;
    public I32     CharacterId;
    public Boolean IsLockedIn;

    //136 bits

    public Team Team => (Team)TeamId;

    public CharacterSelectState(U64 clientId, I32 teamId = -1, I32 characterId = -1, Boolean isLockedIn = false)
    {
        ClientId    = clientId;
        TeamId      = teamId;
        CharacterId = characterId;
        IsLockedIn  = isLockedIn;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref TeamId);
        serializer.SerializeValue(ref CharacterId);
        serializer.SerializeValue(ref IsLockedIn);
    }

    public Boolean Equals(CharacterSelectState other) =>
        ClientId == other.ClientId && TeamId == other.TeamId && CharacterId == other.CharacterId && IsLockedIn == other.IsLockedIn;
}