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
        [HideInInspector]
        [SerializeField] private Boolean showValue = true;
        
        #if ODIN_INSPECTOR
        [field:ReadOnly]
        //[field:ShowIf(nameof(showPlayerInput))]
        #endif
        [field:SerializeField] public F32 Value { get; internal set; }
        
        #if ODIN_INSPECTOR
        [field:LabelText("Action")]
        #endif
        [field:SerializeField]
        public InputActionReference Action { get; [UsedImplicitly] private set; }
        
        [HideInInspector]
        [SerializeField] private Boolean showPlayerInput = true;
        
        #if ODIN_INSPECTOR
        [ShowIf(nameof(showPlayerInput))]
        #endif
        [SerializeField] private PlayerInput playerInput = null;
        

        #if UNITY_EDITOR
        [ContextMenu(itemName: "Toggle Show Player Input")]
        private void ToggleShowPlayerInput()
        {
            //Mark as dirty so that Prefab auto-save will save the changes
            UnityEditor.Undo.RecordObject(this, name: "Change Show Player Input");
            
            showPlayerInput = !showPlayerInput;
            
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject: this);
        }
        
        [ContextMenu(itemName: "Toggle Show Value")]
        private void ToggleShowValue()
        {
            //Mark as dirty so that Prefab auto-save will save the changes
            UnityEditor.Undo.RecordObject(this, name: "Change Show Value");
            
            showValue = !showValue;
            
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject: this);
        }
        #endif

        #if UNITY_EDITOR
        private void Reset()
        {
            this.GetPlayerInput(out playerInput);
        }
        #endif

        private void Awake()
        {
            if(playerInput == null)
            {
                this.GetPlayerInput(out playerInput);
            }
        }

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