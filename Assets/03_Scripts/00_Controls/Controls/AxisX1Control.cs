namespace Bonkers.Controls
{
    using System;

    using JetBrains.Annotations;

    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    using UnityEngine;
    using UnityEngine.InputSystem;

    using F32 = System.Single;
    
    [AddComponentMenu("Bonkers/Controls/Axis Control")]
    public sealed class AxisControl : MonoBehaviour, 
                                      ISettableControl<F32>
    {
        #if ODIN_INSPECTOR
        [field:ReadOnly]
        #endif
        [field:SerializeField] public F32 Value { get; internal set; }
        
        #if ODIN_INSPECTOR
        [field:LabelText("Action")]
        #endif
        [field:SerializeField]
        public InputActionReference Action { get; [UsedImplicitly] private set; }
        
        #if ODIN_INSPECTOR
        [ShowIf(nameof(showPlayerInput))]
        #endif
        [SerializeField] private PlayerInput playerInput = null;
        
        #if ODIN_INSPECTOR
        [InlineEditor]
        #endif
        [SerializeField] private Boolean showPlayerInput = true;

        #if UNITY_EDITOR
        [ContextMenu(itemName: "Toggle Show Player Input")]
        private void ToggleShowPlayerInput()
        {
            //Mark as dirty so that Prefab auto-save will save the changes
            UnityEditor.Undo.RecordObject(this, name: "Change Show Player Input");
            
            showPlayerInput = !showPlayerInput;
            
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject: this);
        }
        #endif

        #if UNITY_EDITOR
        private void Reset()
        {
            this.GetPlayerInput(out playerInput);
        }
        #endif

        private void OnEnable()
        {
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            
            playerInput.onActionTriggered += OnAnyInputCallback;
        }

        private void OnDisable()
        {
            playerInput.onActionTriggered -= OnAnyInputCallback;
        }

        public void OnAnyInputCallback(InputAction.CallbackContext callbackContext)
        {
            if(Action.action == null)
            {
                Debug.Log(message: $"[Warning] {nameof(Action)} is null.", context: this);
                return;
            }
            
            if (Action.action.id != callbackContext.action.id) return;

            if (callbackContext.action.expectedControlType is "Axis" or "Digital" )
            {
                Value = callbackContext.ReadValue<F32>();
            }
            else
            {
                Debug.Log(message: $"[Warning] {nameof(Action)}'s expected control type is not <b>Axis</b>, it's {callbackContext.action.expectedControlType}", context: this);
            }
        }

        void ISettableControl<F32>.SetValue(F32 value)
        {
            Value = value;
        }
    }
}