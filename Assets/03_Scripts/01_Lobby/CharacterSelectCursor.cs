namespace Bonkers.Lobby
{
    using System;

    using JetBrains.Annotations;

    using UnityEngine.InputSystem;
    using UnityEngine;
    
    using F32   = System.Single;
    using F32x2 = UnityEngine.Vector2;

    public sealed class CharacterSelectCursor : MonoBehaviour//,
                                                 //IInputAxisOwner
    {
        //[SerializeField] private PlayerInput playerInput;
        
        [SerializeField] private F32x2 sensitivity = F32x2.one;
        
        // #if UNITY_EDITOR
        // private void Reset()
        // {
        //     playerInput
        // }
        // #endif
        
        [PublicAPI]
        public void OnPoint(InputAction.CallbackContext context)
        {
            transform.position = (context.ReadValue<Vector2>() * sensitivity);
        }

        [PublicAPI]
        public void OnAddPointDelta(InputAction.CallbackContext context)
        {
            Debug.Log("Delta: " + context.ReadValue<Vector2>());
            transform.position = (Vector3)((Vector2)transform.position + (context.ReadValue<Vector2>() * sensitivity));
        }
        
        [PublicAPI]
        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                Debug.Log("Clicked!");
            }
        }
    }
}
