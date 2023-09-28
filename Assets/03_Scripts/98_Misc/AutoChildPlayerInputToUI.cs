namespace Bonkers
{
    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class AutoChildPlayerInputToUI : MonoBehaviour
    {
        [SerializeField] private RectTransform parent;
        
        #if UNITY_EDITOR
        private void Reset()
        {
            // Parent on reset is canvas.
            parent = FindObjectOfType<Canvas>().transform as RectTransform;
        }
        #endif

        [PublicAPI]
        public void OnSpawnPlayer(PlayerInput player)
        {
            Transform __playerTransform = player.transform;
            __playerTransform.SetParent(parent);
            __playerTransform.localPosition = Vector3.zero;
            __playerTransform.localScale    = Vector3.one;
            __playerTransform.localRotation = Quaternion.identity;
        }
    }
}
