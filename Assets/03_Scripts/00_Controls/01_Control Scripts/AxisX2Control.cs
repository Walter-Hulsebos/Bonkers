namespace Bonkers.Controls
{
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.InputSystem;

    using F32   = System.Single;
    using F32x2 = Unity.Mathematics.float2;
    using Bool  = System.Boolean;
    
    [AddComponentMenu(menuName: "Bonkers/Controls/Axis(×2) Control")]
    public sealed class AxisX2Control : MonoBehaviour,
                                        IControl<F32x2>
    {
        #region Variables
        
        #region Actions
        
        #if ODIN_INSPECTOR
        [BoxGroup(group: "Bindings", showLabel: false)]
        [LabelText(text: "Axis(×2) Binding")]
        [ShowIf(condition: nameof(UseSingleBinding))]
        #endif
        [SerializeField] private InputActionReference binding;
        
        
        #if ODIN_INSPECTOR
        [LabelText(text: "Axis (x) Binding")]
        [ShowIf(condition: nameof(useCompositeBindings))]
        [InlineEditor]
        #endif
        [SerializeField] private AxisControl axisX;
        
        #if ODIN_INSPECTOR
        [LabelText(text: "Axis (y) Binding")]
        [ShowIf(condition: nameof(useCompositeBindings))]
        [InlineEditor]
        #endif
        [SerializeField] private AxisControl axisY;
        
        [HideInInspector]
        [SerializeField] private Bool useCompositeBindings = false;
        private Bool UseSingleBinding => !useCompositeBindings;
        
        #if UNITY_EDITOR
        [ContextMenu(itemName: "Toggle Use Composite Binds")]
        private void ToggleUseCompositeBinds()
        {
            //Mark as dirty so that Prefab auto-save will save the changes
            UnityEditor.Undo.RecordObject(objectToUndo: this, name: "Change Use Composite Binds");
            
            useCompositeBindings = !useCompositeBindings;
            
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject: this);
        }
        #endif
        
        #endregion

        #region PlayerInput
        
        #if ODIN_INSPECTOR
        [BoxGroup(group: "Bindings", showLabel: false)]
        [ShowIf(condition: "@"+nameof(showPlayerInput)+"&&"+nameof(UseSingleBinding))]
        #endif
        [SerializeField] private PlayerInput playerInput = null;
        
        [HideInInspector]
        [SerializeField] private Bool showPlayerInput = true;
        
        #if UNITY_EDITOR
        [ContextMenu(itemName: "Toggle Show Player Input")]
        private void ToggleShowPlayerInput()
        {
            //Mark as dirty so that Prefab auto-save will save the changes
            UnityEditor.Undo.RecordObject(objectToUndo: this, name: "Change Show Player Input");
            
            showPlayerInput = !showPlayerInput;
            
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject: this);
        }
        #endif
        
        #endregion
        
        #region Value
        
        #if ODIN_INSPECTOR
        [BoxGroup(@group: "Value", showLabel: false)]
        [ReadOnly]
        [ShowInInspector]
        #endif
        public F32x2 Value
        {
            get
            {
                if (UseSingleBinding) return _valueBackingField;

                if (axisX == null || axisY == null)
                {
                    return F32x2.zero;
                }
                
                return new F32x2(x: axisX.Value, y: axisY.Value);
            }
            internal set
            {
                if (useCompositeBindings)
                {
                    Debug.LogWarning(message: $"Cannot set {nameof(Value)} of {nameof(AxisX2Control)} when {nameof(useCompositeBindings)} is true!!", context: this);
                }
                
                _valueBackingField = value;
            }
        }
        private F32x2 _valueBackingField = F32x2.zero; 
                
        [HideInInspector]
        [SerializeField] private Bool showValue = true;
        
        #if UNITY_EDITOR
        [ContextMenu(itemName: "Toggle Show Value")]
        private void ToggleShowValue()
        {
            //Mark as dirty so that Prefab auto-save will save the changes
            UnityEditor.Undo.RecordObject(objectToUndo: this, name: "Change Show Value");
            
            showValue = !showValue;
            
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject: this);
        }
        #endif
        
        #endregion
        
        #endregion

        #region Methods
        
        #if UNITY_EDITOR
        #if ODIN_INSPECTOR
        [ShowIf(condition: "@"+nameof(axisX)+" == null && "+nameof(axisY)+" == null && "+nameof(useCompositeBindings))]
        [Button(name: "Create Axis Children")]
        #endif
        [ContextMenu(itemName: "Create Axis Children")]
        private void CreateAxisChildren()
        {
            if (axisX == null)
            {
                GameObject __axisXObject = new (name: "Axis (x)")
                {
                    transform =
                    {
                        parent = transform,
                    },
                };

                axisX = __axisXObject.AddComponent<AxisControl>();
            }
            
            if (axisY == null)
            {
                GameObject __axisYObject = new (name: "Axis (y)")
                {
                    transform =
                    {
                        parent = transform,
                    },
                };
                
                axisY = __axisYObject.AddComponent<AxisControl>();
            }
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
            if(useCompositeBindings) return;
                
            if(playerInput == null)
            {
                this.GetPlayerInput(out playerInput);
            }
        }

        private void OnEnable()
        {
            if(useCompositeBindings) return;
            
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            
            playerInput.onActionTriggered += OnAnyInputCallback;
        }

        private void OnDisable()
        {
            if(useCompositeBindings) return;
            
            playerInput.onActionTriggered -= OnAnyInputCallback;
        }

        private void OnAnyInputCallback(InputAction.CallbackContext callbackContext)
        {
            if (binding.action == null)
            {
                Debug.Log(message: $"[Warning] {nameof(binding)} is null.", context: this);
                return;
            }
            
            //Debug.Log($"OnAnyInputCallback: name: {callbackContext.action.name} - type: {callbackContext.action.expectedControlType} - id: {binding.action.id} - valueObj: {callbackContext.ReadValueAsObject()}", context: this);
            
            if (callbackContext.action.id != binding.action.id) return;

            // To set mode (2=analog, 1=digital, 0=digitalNormalized): 2DVector(mode=2)
            if (callbackContext.action.expectedControlType is "2DVector(mode=2)" or "2DVector")
            {
                // ReSharper disable once RedundantCast
                Value = (F32x2)callbackContext.ReadValue<Vector2>();
            }
            else
            {
                Debug.Log(message: $"[Warning] {nameof(binding)}'s expected control type is not <b>2DVector(mode=2)</b>, it's {callbackContext.action.expectedControlType}", context: this);
            }
        }
        
        #endregion

        // private void Update()
        // {
        //     //Debug.Log($"AxisX2Control: {Value}");
        // }
    }
}
