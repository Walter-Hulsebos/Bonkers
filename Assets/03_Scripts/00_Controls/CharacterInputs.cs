namespace Bonkers.Controls
{
    using System.Collections.Generic;
    
    using UnityEngine;
    
    using Unity.Cinemachine;
    
    using KinematicCharacterController.Examples;

    public sealed class CharacterInputs : MonoBehaviour, IInputAxisOwner
    {
        [SerializeField] private ExampleCharacterController characterController;

        [SerializeField] private Transform cameraOverride;
        
        public Transform CameraTransform => (cameraOverride == null) ? Camera.main.transform : cameraOverride;
        
        [Header(header: "Input Axes")]
        [Tooltip(tooltip: "X Axis movement.  Value is -1..1.  Controls the sideways movement")]
        public InputAxis MoveX = InputAxis.DefaultMomentary;

        [Tooltip(tooltip: "Z Axis movement.  Value is -1..1. Controls the forward movement")]
        public InputAxis MoveZ = InputAxis.DefaultMomentary;

        [Tooltip(tooltip: "Jump movement.  Value is 0 or 1. Controls the vertical movement")]
        public InputAxis Jump = InputAxis.DefaultMomentary;

        /// Report the available input axes to the input axis controller.
        /// We use the Input Axis Controller because it works with both the Input package
        /// and the Legacy input system.
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(item: new () { DrivenAxis = () => ref MoveX, Name = "Move X", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(item: new () { DrivenAxis = () => ref MoveZ, Name = "Move Z", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
            axes.Add(item: new () { DrivenAxis = () => ref Jump,  Name  = "Jump" });
        }

        private void Reset()
        {
            characterController = GetComponent<ExampleCharacterController>();

            cameraOverride = transform.parent.GetComponentInChildren<Camera>().transform;
        }
        
        private void Update()
        {
            HandleCharacterInput();
        }
        
        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs
            {
                MoveAxisRight   = MoveX.Value,
                MoveAxisForward = MoveZ.Value, 
                CameraRotation  = CameraTransform.rotation,
                JumpDown        = (Jump.Value > 0.01f),
            };

            // Apply inputs to character
            characterController.SetInputs(ref characterInputs);
        }
    }
}