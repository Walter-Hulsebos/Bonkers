// namespace Bonkers
// {
//     using UnityEngine;
//     
//     using JetBrains.Annotations;
//
//     using Unity.Netcode;
//     
//     using I32 = System.Int32;
//     using U64 = System.UInt64;
//     
//     public static class Players
//     {
//         public static I32 OnlineCount { get; private set; } = 0;
//         public static I32 LocalCount  { get; private set; } = 0;
//         
//         public static I32 TotalCount => OnlineCount + LocalCount;
//
//         private static NetworkManager _networkManager;
//         
//         // static Players()
//         // {
//         //     // Subscribe to on scene changed event
//         //     SceneManager.activeSceneChanged += OnSceneChanged;
//         // }
//
//         [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.SubsystemRegistration)]
//         private static void Init()
//         {
//             OnlineCount = 0;
//             LocalCount  = 0;
//         }
//         
//         [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.AfterSceneLoad)]
//         private static void AfterSceneLoad()
//         {
//             if (_networkManager == null)
//             {
//                 _networkManager                            =  NetworkManager.Singleton;
//                 _networkManager.OnClientConnectedCallback  += OnClientConnected;
//                 _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
//             }
//         }
//         
//         private static void OnClientConnected(U64 clientId)
//         {
//             Debug.Log(message: $"<color=#00ffff>Client {clientId} connected!</color>");
//             //Debug.Log(message: $"<color=cyan> Client {clientId} connected! </color>");
//             OnlineCount++;
//         }
//         
//         private static void OnClientDisconnected(U64 clientId)
//         {
//             Debug.Log(message: $"<color=#00ffff>Client {clientId} disconnected!</color>");
//             //Debug.Log(message: $"<color=cyan> Client {clientId} disconnected! </color>");
//             OnlineCount--;
//         }
//
//         [PublicAPI]
//         public static void AddLocal()
//         {
//             LocalCount++;
//         }
//
//         [PublicAPI]
//         public static void RemoveLocal()
//         {
//             LocalCount--;
//         }
//         
//         [PublicAPI]
//         public static void RemoveLocal(I32 index)
//         {
//             LocalCount--;
//         }
//     }
// }
