using Cysharp.Threading.Tasks;

namespace Bonkers.Controls
{
    using JetBrains.Annotations;

    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    using UnityEngine;
    using UnityEngine.InputSystem;

    using static ButtonState;

    using Bool = System.Boolean;
    
    public enum ButtonState
    {
        Released  = 0,
        Pressed   = 1,
        ThisFrame = 2,
        PressedThisFrame  = Pressed  | ThisFrame,
        ReleasedThisFrame = Released | ThisFrame,
    }
    
    [AddComponentMenu("Bonkers/Controls/Button Control +")]
    public sealed class ButtonControlPlus : MonoBehaviour, 
                                            ISettableControl<ButtonState>
    {
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
        [HideInInspector]
        #endif
        [SerializeField] private Bool showPlayerInput = true;
        
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
        
        #if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
        #endif
        public ButtonState Value { get; internal set; } = Released;
        
        #if ODIN_INSPECTOR
        [ShowInInspector]
        #endif
        public Bool IsPressed => Value == Pressed;
        #if ODIN_INSPECTOR                  
        [ShowInInspector]                   
        #endif                              
        public Bool IsReleased => Value == Released;
        #if ODIN_INSPECTOR                  
        [ShowInInspector]                   
        #endif                              
        public Bool WasPressedThisFrame => Value == PressedThisFrame;
        #if ODIN_INSPECTOR                  
        [ShowInInspector]                   
        #endif                              
        public Bool WasReleasedThisFrame => Value == ReleasedThisFrame;

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
            Debug.Log(message: $"Button {gameObject.name} OnEnable ({Time.frameCount})", context: this);
            
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            
            playerInput.onActionTriggered += OnAnyInputCallback;
        }

        private void OnDisable()
        {
            playerInput.onActionTriggered -= OnAnyInputCallback;
        }

        public async void OnAnyInputCallback(InputAction.CallbackContext callbackContext)
        {
            if(Action.action == null)
            {
                Debug.Log(message: $"[Warning] {nameof(Action)} is null.", context: this);
                return;
            }
            
            if (Action.action.id != callbackContext.action.id) return;

            if (callbackContext.action.expectedControlType is "Button")
            {
                if (callbackContext.action.WasPressedThisFrame())
                {
                    Value = PressedThisFrame;
                    Debug.Log(message: $"Button {gameObject.name} WasPressedThisFrame ({Time.frameCount})", context: this);

                    //I don't understand why, but we need two yields here.
                    await UniTask.Yield();
                    await UniTask.Yield();
                    
                    Value = Pressed;
                    Debug.Log(message: $"Button {gameObject.name} Pressed ({Time.frameCount})", context: this);
                }
                else if (callbackContext.action.WasReleasedThisFrame())
                {
                    Value = ReleasedThisFrame;
                    Debug.Log(message: $"Button {gameObject.name} WasReleasedThisFrame ({Time.frameCount})", context: this);
                    
                    //I don't understand why, but we need two yields here.
                    await UniTask.Yield();
                    await UniTask.Yield();
                    
                    Value = Released;
                    Debug.Log(message: $"Button {gameObject.name} Released ({Time.frameCount})", context: this);
                }
                //NOTE: [Walter] Can't have checks for Pressed and Released here, because playerInput.onActionTriggered only happens on changes (WasPressedThisFrame and WasReleasedThisFrame for Buttons) .
            }
            else
            {
                Debug.Log(message: $"[Warning] {nameof(Action)}'s expected control type is not <b>Button</b>, it's {callbackContext.action.expectedControlType}", context: this);
            }
        }
    }
}