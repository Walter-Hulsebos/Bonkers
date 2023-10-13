using UnityEngine;

namespace KinematicCharacterController.Examples
{
    using System;

    using UnityEngine.InputSystem;

    public class ExamplePlayer : MonoBehaviour
    {
        public ExampleCharacterController Character;
        //public ExampleCharacterCamera CharacterCamera;

        public Transform cameraTransform;
        
        // private Vector2 lookInput;
        // public void SetLookInput(InputAction.CallbackContext context)
        // {
        //     lookInput = context.ReadValue<Vector2>();
        // }
            
        private Vector2 moveInput;
        public void SetMoveInput(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        
        private Boolean jumpInput;
        public void SetJumpInput(InputAction.CallbackContext context)
        {
            jumpInput = context.ReadValueAsButton();
        }
        
        private void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;

            // // Tell camera to follow transform
            // CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);
            //
            // // Ignore the character's collider(s) for camera obstruction checks
            // CharacterCamera.IgnoredColliders.Clear();
            // CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            // if (Input.GetMouseButtonDown(0))
            // {
            //     Cursor.lockState = CursorLockMode.Locked;
            // }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            //TODO: Handle this for Cinemachine!
            // if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            // {
            //     CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
            //     CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            // }

            //HandleCameraInput();
        }

        // private void HandleCameraInput()
        // {
        //     // Create the look input vector for the camera
        //     // float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
        //     // float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
        //     //Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);
        //     Vector3 lookInputVector = new(lookInput.x, lookInput.y, 0f);
        //
        //     // Prevent moving the camera while the cursor isn't locked
        //     if (Cursor.lockState != CursorLockMode.Locked)
        //     {
        //         lookInputVector = Vector3.zero;
        //     }
        //
        //     // Input for zooming the camera (disabled in WebGL because it can cause problems)
        //     //float scrollInput = -Input.GetAxis(MouseScrollInput);
        //     // #if UNITY_WEBGL
        //     //         scrollInput = 0f;
        //     // #endif
        //
        //     // Apply inputs to the camera
        //     //CharacterCamera.UpdateWithInput(Time.deltaTime, zoomInput: 0, lookInputVector);
        //
        //     // Handle toggling zoom level
        //     // if (Input.GetMouseButtonDown(1))
        //     // {
        //     //     CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
        //     // }
        // }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs
            {
                MoveAxisForward = moveInput.y, 
                MoveAxisRight   = moveInput.x,
                CameraRotation  = cameraTransform.rotation,
                JumpDown        = jumpInput,
            };

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
    }
}