using I32 = System.Int32;

namespace Bonkers.Lobby
{
    using JetBrains.Annotations;

    using UnityEngine;

    public class PlayerCounter : MonoBehaviour
    {
        public static I32 playerCount;

        [PublicAPI]
        public void PlayerJoined() => playerCount++;

        [PublicAPI]
        public void PlayerLeft() => playerCount--;  
    }
}