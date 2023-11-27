using System;

using Unity.Netcode;

namespace Bonkers.Lobby
{
    public struct CharacterSelectState : INetworkSerializable,
                                         IEquatable<CharacterSelectState>
    {
        public UInt64 ClientId;
        public Int32  CharacterId;

        public CharacterSelectState(UInt64 clientId, Int32 characterId = -1)
        {
            ClientId    = clientId;
            CharacterId = characterId;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref CharacterId);
        }

        public Boolean Equals(CharacterSelectState other) => ClientId == other.ClientId && CharacterId == other.CharacterId;
    }
}