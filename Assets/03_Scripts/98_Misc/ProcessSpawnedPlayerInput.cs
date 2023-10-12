namespace Bonkers
{
    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.InputSystem;
    
    using UltEvents;

    public sealed class ProcessSpawnedPlayerInput : MonoBehaviour
    {
        [SerializeField] private UltEvent<PlayerInput>[] actionsOnJoin;

        [SerializeField] private UltEvent<PlayerInput>[] actionsOnLeave;
        
        [PublicAPI]
        public void PlayerJoined(PlayerInput playerInput)
        {
            foreach (UltEvent<PlayerInput> __action in actionsOnJoin)
            {
                __action.Invoke(playerInput);
            }
        }

        [PublicAPI]
        public void PlayerLeft(PlayerInput playerInput)
        {
            foreach (UltEvent<PlayerInput> __action in actionsOnLeave)
            {
                __action.Invoke(playerInput);
            }
        }  
    }
}
