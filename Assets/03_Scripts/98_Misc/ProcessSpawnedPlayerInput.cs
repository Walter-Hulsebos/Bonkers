namespace Bonkers
{
    using System;

    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.InputSystem;
    
    using UltEvents;

    public sealed class ProcessSpawnedPlayerInput : MonoBehaviour
    {
        [SerializeField] private PlayerInputManager    playerInputManager;
        [SerializeField] private UltEvent<PlayerInput> actionsOnJoin;
        [SerializeField] private UltEvent<PlayerInput> actionsOnLeave;

        #if UNITY_EDITOR
        private void Reset()
        {
            playerInputManager = GetComponent<PlayerInputManager>();
        }
        #endif

        private void OnEnable()
        {
            if (playerInputManager == null) return;

            playerInputManager.onPlayerJoined += PlayerJoined;
        }
        
        private void OnDisable()
        {
            if (playerInputManager == null) return;
            
            playerInputManager.onPlayerLeft += PlayerLeft;
        }

        [PublicAPI]
        public void PlayerJoined(PlayerInput playerInput)
        {
            actionsOnJoin.Invoke(playerInput);
        }

        [PublicAPI]
        public void PlayerLeft(PlayerInput playerInput)
        {
            actionsOnLeave.Invoke(playerInput);
        }
        
    }
}