namespace Bonkers.Controls
{
    using System;

    using Unity.Cinemachine;

    using UnityEngine;
    using UnityEngine.InputSystem;
    
    using F32 = System.Single;

    // This class receives input from a PlayerInput component and dispatches it
    // to the appropriate Cinemachine InputAxis.
    // The playerInput component should be on the same GameObject, or specified in the PlayerInput field.
    public class CinemachineInputHandler : InputAxisControllerBase<CinemachineInputHandler.Reader>
    {
        [Header(header: "Input Source Override")]
        public PlayerInput playerInput;

        private void Awake()
        {
            // When the PlayerInput receives an input, send it to all the controllers
            if (playerInput == null)
            {
                Debug.Log(message: "PlayerInput not specified, looking for one on this and parent GameObject", context: this);
                
                if (!TryGetComponent(component: out playerInput))
                {
                    if (!transform.parent.TryGetComponent(component: out playerInput))
                    {
                        playerInput = GetComponentInChildren<PlayerInput>();
                    }
                }
            }

            if (playerInput == null)
            {
                Debug.LogError(message: "Cannot find PlayerInput component", context: this);
            }
            else
            {
                playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
                playerInput.onActionTriggered += (value) =>
                {
                    foreach (Controller __controller in Controllers)
                    {
                        __controller.Input.ProcessInput(action: value.action);
                    }
                };
            }
        }

        // We process user input on the Update clock
        private void Update()
        {
            if (Application.isPlaying)
                UpdateControllers();
        }

        // Controllers will be instances of this class.
        [Serializable]
        public class Reader : IInputAxisReader
        {
            public  InputActionReference input;
            private Vector2              _value; // the cached value of the input

            public void ProcessInput(InputAction action)
            {
                // If it's my action then cache the new value
                if (input != null && input.action.id == action.id)
                {
                    if (action.expectedControlType == "Vector2")
                        _value = action.ReadValue<Vector2>();
                    else
                        _value.x = _value.y = action.ReadValue<F32>();
                }
            }

            // IInputAxisReader interface: Called by the framework to read the input value
            public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
            {
                return (hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? _value.y : _value.x);
            }
        }
    }
}