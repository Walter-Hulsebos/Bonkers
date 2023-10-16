// namespace Bonkers.Controls
// {
//     using System;
//
//     using JetBrains.Annotations;
//
//     using Sirenix.OdinInspector;
//
//     using UnityEngine;
//     using UnityEngine.InputSystem;
//     
//     using F32   = System.Single;
//     using F32x2 = Unity.Mathematics.float2;
//
//     public sealed class ControlsUpdateSystem : MonoBehaviour
//     {
//         //[field:SerializeReference]
//         [ShowInInspector]
//         public ISettableControl<F32>[] Axes { get; [UsedImplicitly] private set; } = Array.Empty<ISettableControl<F32>>();
//         
//         [SerializeField] private PlayerInput playerInput = null;
//
//         #if UNITY_EDITOR
//         private void Reset()
//         {
//             playerInput = GetComponent<PlayerInput>();
//             
//             if (playerInput == null)
//             {
//                 Debug.Log($"PlayerInput component not found on {gameObject.name}.", context: this);
//             }
//
//             Axes = GetComponentsInChildren<ISettableControl<F32>>();
//         }
//         #endif
//         
//         private void Awake()
//         {
//             // Make sure we have a PlayerInput component
//             if (playerInput == null)
//             {
//                 Debug.Log(message: "PlayerInput not specified, looking for one on this and parent GameObject", context: this);
//                 
//                 if (!TryGetComponent(component: out playerInput))
//                 {
//                     if (!transform.parent.TryGetComponent(component: out playerInput))
//                     {
//                         playerInput = GetComponentInChildren<PlayerInput>();
//                     }
//                 }
//             }
//             
//             // Make sure the PlayerInput component is set to invoke C# events
//             playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
//         }
//
//         private void OnEnable()
//         {
//             foreach (ISettableControl<F32> __axis in Axes)
//             {
//                 playerInput.onActionTriggered += __axis.OnAnyInputCallback; 
//             } 
//         }
//         
//         private void OnDisable()
//         {
//             //NOTE: This is not safe if the Axes array is modified during runtime, but that should never happen.
//             foreach (ISettableControl<F32> __axis in Axes)
//             {
//                 playerInput.onActionTriggered -= __axis.OnAnyInputCallback; 
//             } 
//         }
//     }
// }
