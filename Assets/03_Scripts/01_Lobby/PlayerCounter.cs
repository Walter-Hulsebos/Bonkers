using I32 = System.Int32;

namespace Bonkers.Lobby
{
    using JetBrains.Annotations;

    using Sirenix.OdinInspector;

    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class PlayerCounter : MonoBehaviour
    {
        [ShowInInspector]
        public static I32 playerCount;
        
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            playerCount = 0;
        }

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