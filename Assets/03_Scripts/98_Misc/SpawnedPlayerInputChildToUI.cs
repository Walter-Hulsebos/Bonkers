namespace Bonkers
{
    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class SpawnedPlayerInputChildToUI : MonoBehaviour
    {
        [SerializeField] private RectTransform parent;
        
        [SerializeField] private PlayerInputManager playerInputManager;
        
        #if UNITY_EDITOR
        private void Reset()
        {
            // Parent on reset is canvas.
            parent             = FindObjectOfType<Canvas>().transform as RectTransform;
            playerInputManager = GetComponent<PlayerInputManager>();
        }
        #endif
        
        private void OnEnable()
        {
            if (playerInputManager == null) return;

            playerInputManager.onPlayerJoined += OnSpawnPlayer;
        }
        
        private void OnDisable()
        {
            if (playerInputManager == null) return;
            
            playerInputManager.onPlayerLeft += OnSpawnPlayer;
        }

        [PublicAPI]
        public void OnSpawnPlayer(PlayerInput player)
        {
            Debug.Log($"Spawned player {player.playerIndex}, setting parent to {parent.name}");
            
            Transform __playerTransform = player.transform;
            __playerTransform.SetParent(parent);
            __playerTransform.localPosition = Vector3.zero;
            __playerTransform.localScale    = Vector3.one;
            __playerTransform.localRotation = Quaternion.identity;
        }
    }
}
