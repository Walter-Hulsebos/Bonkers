namespace Bonkers
{
    using Unity.Netcode;
    using UnityEngine;

    using Bool = System.Boolean;

    public static class CharacterSelectStatesExtensions
    {
        public static Bool TryGetLocal(this NetworkList<CharacterSelectState> players, out CharacterSelectState localPlayer)
        {
            foreach (CharacterSelectState __player in players)
            {
                if (__player.ClientId != NetworkManager.Singleton.LocalClientId) continue;

                localPlayer = __player;
                return true;
            }

            Debug.LogError("Could not find local player in players list");
            localPlayer = default(CharacterSelectState);
            return false;
        }
    }
}
