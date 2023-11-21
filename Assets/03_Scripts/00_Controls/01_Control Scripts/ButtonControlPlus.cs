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
        Released = 0,
        Pressed = 1,
        ThisFrame = 2,
        PressedThisFrame = Pressed | ThisFrame,
        ReleasedThisFrame = Released | ThisFrame,
    }

    [AddComponentMenu("Bonkers/Controls/Button Control +")]
    public sealed class ButtonControlPlus : MonoBehaviour,
                                        ISettableControl<ButtonState>
    {
#if ODIN_INSPECTOR
        [field: LabelText("Action")]
#endif
        [field: SerializeField]
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
        [field: ReadOnly]
        [ShowInInspector]
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
            if (playerInput == null)
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
            if (Action.action == null)
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
                }
                else if (callbackContext.action.WasReleasedThisFrame())
                {
                    Value = ReleasedThisFrame;
                }
                else
                {
                    Value = callbackContext.action.IsPressed() ? Pressed : Released;
                }
            }
            else
            {
                Debug.Log(message: $"[Warning] {nameof(Action)}'s expected control type is not <b>Axis</b>, it's {callbackContext.action.expectedControlType}", context: this);
            }
        }
    }
}