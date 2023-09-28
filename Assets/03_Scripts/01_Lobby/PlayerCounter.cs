using I32 = System.Int32;

namespace Bonkers.Lobby
{
    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class PlayerCounter : MonoBehaviour
    {
        public static I32 playerCount;

        [PublicAPI]
        public void PlayerJoined(PlayerInput playerInput)
        {
            playerCount++;
        }

        [PublicAPI]
        public void PlayerLeft(PlayerInput playerInput)
        {
            playerCount--;
        }  
    }
}