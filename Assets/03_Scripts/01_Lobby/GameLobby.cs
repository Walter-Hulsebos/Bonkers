using System;
using System.Collections.Generic;
using System.Text;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using UnityEngine;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

using JetBrains.Annotations;

using QFSW.QC;

using F32 = System.Single;
using I32 = System.Int32;
using U16 = System.UInt16;

namespace Bonkers.Lobby
{
    using CGTK.Utils.Extensions.NetCode;

    using Lobby = Unity.Services.Lobbies.Models.Lobby;

    public static class GameLobby
    {
        /// <summary> Resets the GameLobby's static variables when the Domain is reloaded. (for Configurable Enter Play Mode) </summary>
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Debug.Log(message: "Resetting GameLobby static variables to defaults.");
            
            _heartbeatTimer = 0f;
            _joinedLobby = null;

            //Unsubscribe all listeners
            OnLobbyListChanged = null;  
        }
        
        private const  String KEY_RELAY_JOIN_CODE = "Rughaar";
        private static F32    _heartbeatTimer;
        
        private static Lobby _joinedLobby;
        //private static F32   _listLobbiesTimer;

        // public static event EventHandler OnCreateLobbyStarted;
        // public static event EventHandler OnCreateLobbyFailed;
        //
        // public static event EventHandler OnJoinStarted;
        // public static event EventHandler OnQuickJoinFailed;
        // public static event EventHandler OnJoinFailed;

        public static event Action<List<Lobby>> OnLobbyListChanged;

        public static Boolean        HasManager   => Manager != null;
        public static Boolean        HasNoManager => !HasManager;
        public static NetworkManager Manager      => NetworkManager.Singleton;


        private static UnityTransport _cachedTransport;
        public static  Boolean        HasTransport   => _cachedTransport != null;
        public static  Boolean        HasNoTransport => !HasTransport;

        public static UnityTransport Transport
        {
            get
            {
                if (HasTransport) return _cachedTransport;

                if (HasManager)
                {
                    _cachedTransport = Manager.GetComponent<UnityTransport>();

                    if (_cachedTransport == null) { Debug.LogError(message: "No UnityTransport on the NetworkManager!"); }
                }
                else { Debug.LogError(message: "No NetworkManager in the scene!"); }

                return _cachedTransport;
            }
        }

        static GameLobby()
        {
            SceneManager.activeSceneChanged += ActiveSceneChanged;

            //Check if in play mode
            if (Application.isPlaying) { InitializeUnityAuthentication().Forget(); }
        }

        private static async void ActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            Debug.Log(message: $"Active scene changed from {oldScene.name} to {newScene.name}");

            //Wait for 1 frame to ensure that the NetworkManager has been initialized
            await UniTask.NextFrame();

            await InitializeUnityAuthentication();
        }

        public static async UniTask InitializeUnityAuthentication()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions __initializationOptions = new();

                Debug.Log(message: "Initializing Unity Services...");

                await UnityServices.InitializeAsync(options: __initializationOptions);

                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                Debug.Log(message: $"Signed in as {AuthenticationService.Instance.PlayerId}");
            }
        }

        [Command]
        public static async void Host(String name, I32 teamSizes = 3)
        {
            try
            {
                I32 __maxConnections = teamSizes * 2;
                
                Allocation __hostAlloc = await RelayService.Instance.CreateAllocationAsync(maxConnections: __maxConnections);

                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocationId: __hostAlloc.AllocationId);

                Transport.SetHostRelayDataFromAlloc(alloc: __hostAlloc);

                if (NetworkManager.Singleton.StartHost())
                {
                    Debug.Log(message: $"<color=#00ff00ff>Hosting lobby {name} with join code </color><b>{JoinCode}</b><color=#00ff00ff>, max connections: {__maxConnections}</color>");
                }
                else
                {
                    Debug.LogError(message: "Failed to start host!");
                }
            }
            catch (LobbyServiceException __serviceException)
            {
                Debug.Log(message: __serviceException);
            }
        }

        [Command]
        public static async void Join(String joinCode)
        {
            JoinAllocation __joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);

            Transport.SetClientRelayDataFromAlloc(alloc: __joinAlloc);

            NetworkManager.Singleton.StartClient();
        }

        // [Command]
        // public static async void QuickJoin()
        // {
        //     try
        //     {
        //         // Attempt to join a lobby with an open slot.
        //         Lobby __lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
        //         
        //         if (__lobby == null)
        //         {
        //             Debug.Log(message: "No lobbies found!");
        //             return;
        //         }
        //
        //         JoinAllocation __joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode: __lobby.Data[key: KEY_RELAY_JOIN_CODE].Value);
        //         
        //         Transport.SetClientRelayDataFromAlloc(alloc: __joinAlloc);
        //
        //         NetworkManager.Singleton.StartClient();
        //         
        //         _joinedLobby = __lobby;
        //     }
        //     catch (Exception __exception)
        //     {
        //         Debug.Log(message: $"No lobbies found via quick join! {__exception}");
        //     }
        // }

        [Command]
        public static String JoinCode { [UsedImplicitly] get; private set; }

        [Command]
        public static void Leave() { }

        //TODO: Close server when everyone leaves.

        [Command]
        public static async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions __queryLobbiesOptions = new()
                {
                    Filters = new List<QueryFilter>
                    {
                        new(field: QueryFilter.FieldOptions.AvailableSlots, value: "0", op: QueryFilter.OpOptions.GT),
                    },
                };

                QueryResponse __queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options: __queryLobbiesOptions);

                OnLobbyListChanged?.Invoke(obj: __queryResponse.Results);

                if (__queryResponse.Results == null || __queryResponse.Results.Count == 0)
                {
                    Debug.Log(message: "No lobbies found!");
                    return;
                }

                StringBuilder __stringBuilder = new();

                foreach (Lobby __lobby in __queryResponse.Results)
                {
                    __stringBuilder.AppendLine(value: $"Lobby: {__lobby.Id} - [{__lobby.AvailableSlots}/{__lobby.MaxPlayers}]");
                }

                Debug.Log(message: __stringBuilder.ToString());
            }
            catch (LobbyServiceException __e) { Debug.Log(message: __e); }
        }

        [Command]
        public static Lobby MyLobby() => _joinedLobby;

        private static void HandleHeartbeat()
        {
            if (!IsLobbyHost) return;

            _heartbeatTimer -= Time.deltaTime;

            if (_heartbeatTimer > 0f) return;

            const F32 HEARTBEAT_TIMER_MAX = 15f;
            _heartbeatTimer = HEARTBEAT_TIMER_MAX;

            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId: _joinedLobby.Id);
        }

        private static Boolean IsLobbyHost => _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
}