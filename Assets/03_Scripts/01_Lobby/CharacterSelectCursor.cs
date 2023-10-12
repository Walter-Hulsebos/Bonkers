namespace Bonkers.Lobby
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Sirenix.OdinInspector;

    using Unity.Cinemachine;

    using UnityEngine.InputSystem;
    using UnityEngine;
    using UnityEngine.InputSystem.LowLevel;

    using static UnityEngine.Mathf;

    using F32   = System.Single;
    using F32x2 = UnityEngine.Vector2;
    using F64   = System.Double;

    public sealed class CharacterSelectCursor : MonoBehaviour, IInputAxisOwner
    {
        //[SerializeField] private PlayerInput playerInput;
        
        // #if ODIN_INSPECTOR
        // [BoxGroup("Cursor", showLabel: false)]
        // #endif
        // [SerializeField] private F32x2 sensitivity = F32x2.one;

        #if ODIN_INSPECTOR
        [BoxGroup("Cursor", showLabel: false)]
        #endif
        [SerializeField] private F32 cursorSpeed = 800;
        
        #if ODIN_INSPECTOR
        [BoxGroup("Cursor", showLabel: false)]
        #endif
        [SerializeField] private Canvas canvas;
        
        #if ODIN_INSPECTOR
        [BoxGroup("Cursor", showLabel: false)]
        #endif
        [SerializeField] private RectTransform cursorTransform;

        #if ODIN_INSPECTOR
        [FoldoutGroup("Inputs")]
        #endif
        [Tooltip(tooltip: "X Axis point.  Value is -1..1.")]
        public InputAxis PointX = InputAxis.DefaultMomentary;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Inputs")]
        #endif
        [Tooltip(tooltip: "Y Axis point.  Value is -1..1.")]
        public InputAxis PointY = InputAxis.DefaultMomentary;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Inputs")]
        #endif
        [Tooltip(tooltip: "X Axis stick.  Value is -1..1.")]
        public InputAxis StickX = InputAxis.DefaultMomentary;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Inputs")]
        #endif
        [Tooltip(tooltip: "Y Axis stick.  Value is -1..1.")]
        public InputAxis StickY = InputAxis.DefaultMomentary;

        #if ODIN_INSPECTOR
        [FoldoutGroup("Inputs")]
        #endif
        [Tooltip(tooltip: "Value is 0 or 1.")]
        public InputAxis Click = InputAxis.DefaultMomentary;
        
        /// Report the available input axes to the input axis controller.
        /// We use the Input Axis Controller because it works with both the Input package
        /// and the Legacy input system.
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            // axes.Add(item: new IInputAxisOwner.AxisDescriptor()
            // {
            //     DrivenAxis = () => ref PointX, 
            //     Name = "Point X", 
            //     Hint = IInputAxisOwner.AxisDescriptor.Hints.X,
            // });
            // axes.Add(item: new IInputAxisOwner.AxisDescriptor()
            // {
            //     DrivenAxis = () => ref PointY, 
            //     Name = "Point Y", 
            //     Hint = IInputAxisOwner.AxisDescriptor.Hints.Y,
            // });
            
            axes.Add(item: new IInputAxisOwner.AxisDescriptor()
            {
                DrivenAxis = () => ref StickX, 
                Name = "Stick X", 
                Hint = IInputAxisOwner.AxisDescriptor.Hints.X,
            });
            axes.Add(item: new IInputAxisOwner.AxisDescriptor()
            {
                DrivenAxis = () => ref StickY, 
                Name = "Stick Y", 
                Hint = IInputAxisOwner.AxisDescriptor.Hints.Y,
            });
            
            axes.Add(item: new IInputAxisOwner.AxisDescriptor()
            {
                DrivenAxis = () => ref Click, 
                Name = "Click", 
                Hint = IInputAxisOwner.AxisDescriptor.Hints.Default,
            });
        }

        
        #if UNITY_EDITOR
        private void Reset()
        {
            //playerInput = GetComponent<PlayerInput>();

            cursorTransform = (RectTransform)transform;
        }
        #endif

        private void Start()
        {
            if (canvas == null)
            {
                canvas = FindObjectOfType<Canvas>(); 
            }
        }

        private void Update()
        {
            OnMoveStick(stickValue: new Vector2(x: StickX.Value, y: StickY.Value));
            
            OnPoint(pointValue: new Vector2(x: PointX.Value, y: PointY.Value));
            
            OnClick(clickValue: Click.Value);
        }

        [PublicAPI]
        public void OnPoint(InputAction.CallbackContext context)
        {
            cursorTransform.anchoredPosition = (context.ReadValue<F32x2>() * cursorSpeed * Time.deltaTime);
        }
        
        [PublicAPI]
        public void OnPoint(Vector2 pointValue)
        {
            cursorTransform.anchoredPosition = (pointValue);
        }

        private F64   _lastTime;
        private F32x2 _lastStickValue;
        public void OnMoveStick(Vector2 stickValue)
        {
            if (Approximately(a: 0, b: stickValue.x) && Approximately(a: 0, b: stickValue.y))
            {
                // Motion has stopped.
                _lastTime       = default;
                _lastStickValue = default;
            }
            else
            {
                F64 __currentTime = InputState.currentTime;
                if (Approximately(a: 0, b: _lastStickValue.x) && Approximately(a: 0, b: _lastStickValue.y))
                {
                    // Motion has started.
                    _lastTime = __currentTime;
                }

                // Compute delta.
                F32     __deltaTime = (F32)(__currentTime - _lastTime);
                Vector2 __delta     = new (x: cursorSpeed * stickValue.x * __deltaTime, y: cursorSpeed * stickValue.y * __deltaTime);

                // Update position.
                //var currentPosition = m_VirtualMouse.position.value;
                Vector2 __currentPosition = (Vector2)cursorTransform.anchoredPosition;
                Vector2 __newPosition = __currentPosition + __delta;
                
                // Clamp to canvas.
                if (canvas != null)
                {
                    // Clamp to canvas.
                    var __pixelRect = canvas.pixelRect;
                    __newPosition.x = Clamp(value: __newPosition.x, min: __pixelRect.xMin, max: __pixelRect.xMax);
                    __newPosition.y = Clamp(value: __newPosition.y, min: __pixelRect.yMin, max: __pixelRect.yMax);
                }
                
                cursorTransform.anchoredPosition = __newPosition;

                _lastStickValue = stickValue;
                _lastTime = __currentTime;

                // // Update hardware cursor.
                // m_SystemMouse?.WarpCursorPosition(__newPosition);
            }
        }

        // private F64   _lastTime;
        // private F32x2 _lastStickValue;
        //
        // [PublicAPI]
        // public void OnAddPointDelta(InputAction.CallbackContext context)
        // {
        //     // Debug.Log("Delta: " + context.ReadValue<Vector2>());
        //     // transform.position = (Vector3)((Vector2)transform.position + (context.ReadValue<Vector2>() * sensitivity));
        //     
        //     F32x2 __delta = context.ReadValue<F32x2>();
        //     
        //     Debug.Log(message: "Stick value: " + __delta);
        //
        //     Boolean __motionHasStopped = Approximately(a: __delta.x, b: 0) && Approximately(a: __delta.y, b: 0);
        //
        //     if (__motionHasStopped) return;
        //     
        //     Vector2 __currentPosition = (Vector2)cursorTransform.anchoredPosition;
        //     Vector2 __newPosition     = __currentPosition + __delta * sensitivity * Time.deltaTime;
        //     
        //     if (canvas != null)
        //     {
        //         // Clamp to canvas.
        //         Rect __pixelRect = canvas.pixelRect;
        //         
        //         
        //         __newPosition.x = Clamp(value: __newPosition.x, min: __pixelRect.xMin - (__pixelRect.width  * 0.5f), max: __pixelRect.xMax - (__pixelRect.width  * 0.5f));
        //         __newPosition.y = Clamp(value: __newPosition.y, min: __pixelRect.yMin - (__pixelRect.height * 0.5f), max: __pixelRect.yMax - (__pixelRect.height * 0.5f));
        //     }
        //     
        //     cursorTransform.anchoredPosition = __newPosition;
        //     
        //     
        //
        //     // if (__motionHasStopped)
        //     // {
        //     //     _lastStickValue = stickValue;
        //     //     _lastTime       = InputState.currentTime;
        //     // }
        //     // else
        //     // {
        //     //     F64 __currentTime = InputState.currentTime;
        //     //     if (Approximately(0, _lastStickValue.x) && Approximately(0, _lastStickValue.y))
        //     //     {
        //     //         // Motion has started.
        //     //         _lastTime = __currentTime;
        //     //     }
        //         
        //         // Compute delta.
        //         // F32 __deltaTime = (F32)(__currentTime - _lastTime);
        //         // Vector2 __delta = new
        //         // (
        //         //     x: stickValue.x * deltaSensitivity.x * __deltaTime, 
        //         //     y: stickValue.y * deltaSensitivity.y * __deltaTime
        //         // );
        //         
        //         
        //
        //         // Update position.
        //         // Vector2 __currentPosition = (Vector2)cursorTransform.anchoredPosition;
        //         // Vector2 __newPosition     = __currentPosition + __delta;
        //         
        //         // Clamp to canvas.
        //         // if (canvas != null)
        //         // {
        //         //     // Clamp to canvas.
        //         //     Rect __pixelRect = canvas.pixelRect;
        //         //     __newPosition.x = Clamp(__newPosition.x, __pixelRect.xMin, __pixelRect.xMax);
        //         //     __newPosition.y = Clamp(__newPosition.y, __pixelRect.yMin, __pixelRect.yMax);
        //         // }
        //
        //         ////REVIEW: the fact we have no events on these means that actions won't have an event ID to go by; problem?
        //         // InputState.Change(m_VirtualMouse.position, __newPosition);
        //         // InputState.Change(m_VirtualMouse.delta, __delta);
        //
        //         // Update software cursor transform, if any.
        //         // cursorTransform.anchoredPosition = __newPosition;
        //         //
        //         // _lastStickValue = stickValue;
        //         // _lastTime       = __currentTime;
        //
        //         // Update hardware cursor.
        //         //m_SystemMouse?.WarpCursorPosition(__newPosition);
        //     //}
        // }
        
        [PublicAPI]
        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                Debug.Log(message: "Clicked!");
            }
        }
        
        [PublicAPI]
        public void OnClick(F32 clickValue)
        {
            if (clickValue > 0)
            {
                Debug.Log(message: "Clicked!");
            }
        }
    }
}
