namespace Bonkers.Lobby
{
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

    using Cysharp.Threading.Tasks;

    #if UNITY_EDITOR
    using ParrelSync;
    #endif

    using QFSW.QC;
    
    using CGTK.Utils.Extensions.NetCode;

    using JetBrains.Annotations;

    using static Unity.Services.Lobbies.Models.DataObject.VisibilityOptions;

    using Lobby = Unity.Services.Lobbies.Models.Lobby;

    using F32 = System.Single;
    using I32 = System.Int32;
    using U16 = System.UInt16;
    
    using Bool = System.Boolean;

    [PublicAPI]
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
        private static F32   _heartbeatTimer;
        private static Lobby _joinedLobby;
        public static event Action<List<Lobby>> OnLobbyListChanged;

        public static Boolean        HasManager   => Manager != null;
        public static Boolean        HasNoManager => !HasManager;
        public static NetworkManager Manager      => NetworkManager.Singleton;


        private static UnityTransport _cachedTransport;
        public static  Boolean        HasTransport   => _cachedTransport != null;
        public static  Boolean        HasNoTransport => !HasTransport;
        public static UnityTransport  Transport
        {
            get
            {
                if (HasTransport) return _cachedTransport;

                if (HasManager)
                {
                    _cachedTransport = Manager.GetComponent<UnityTransport>();

                    if (_cachedTransport == null) { Debug.LogError(message: "No UnityTransport on the NetworkManager!"); }
                }
                else
                {
                    Debug.LogError(message: "No NetworkManager in the scene!");
                }

                return _cachedTransport;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void AfterSceneLoad()
        {
            await Authenticate();
        }

        public static async UniTask Authenticate()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;
            
            InitializationOptions __initializationOptions = new();

            #if UNITY_EDITOR
            Bool __isClone = ClonesManager.IsClone();
            
            __initializationOptions.SetProfile(__isClone ? ClonesManager.GetArgument() : "Primary");
            #endif
            
            Debug.Log(message: "Initializing Unity Services...");
            await UnityServices.InitializeAsync(options: __initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log(message: $"Signed in as {AuthenticationService.Instance.PlayerId}");
        }

        [Command(aliasOverride: "host")]
        public static async void Host(String name, I32 teamSizes = 3)
        {
            try
            {
                I32 __maxConnections = teamSizes * 2;
                
                Allocation __hostAlloc = await RelayService.Instance.CreateAllocationAsync(maxConnections: __maxConnections);
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocationId: __hostAlloc.AllocationId);
                
                CreateLobbyOptions __options = new()
                {
                    Data = new Dictionary<String, DataObject>
                    {
                        { 
                            KEY_RELAY_JOIN_CODE, 
                            new DataObject(visibility: Public, value: _joinCode) 
                        },
                    },
                };
                
                Lobby __lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName: name, maxPlayers: __maxConnections, options: __options);
                
                // Start a heartbeat timer to keep the lobby alive. 
                
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

        [Command(aliasOverride: "join")]
        public static async void Join(String joinCode)
        {
            JoinAllocation __joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);

            Transport.SetClientRelayDataFromAlloc(alloc: __joinAlloc);

            NetworkManager.Singleton.StartClient();
        }

        [Command(aliasOverride: "quick-join")]
        public static async UniTask<Lobby> QuickJoin()
        {
            try
            {
                // Attempt to join a lobby in progress
                QuickJoinLobbyOptions __quickJoinOptions = new() 
                { 
                    Filter = new List<QueryFilter>
                    {
                        new (field: QueryFilter.FieldOptions.AvailableSlots, value: "0",     op: QueryFilter.OpOptions.GT),
                        new (field: QueryFilter.FieldOptions.IsLocked,       value: "false", op: QueryFilter.OpOptions.EQ),
                    }, 
                };

                Lobby __lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options: __quickJoinOptions);

                if (__lobby == null)
                {
                    Debug.Log(message: $"No lobbies found. Retrying in 3 seconds...");
                    await UniTask.Delay(millisecondsDelay: 1000);
                    Debug.Log(message: "2");
                    await UniTask.Delay(millisecondsDelay: 1000);
                    Debug.Log(message: "1");
                }
                
                __lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options: __quickJoinOptions);
                
                if (__lobby == null)
                {
                    Debug.LogWarning(message: $"No lobbies found after second atempt, <b>aborting</b>!");
                }
                
                
                
                // If we found one grab the relay allocation details.
                JoinAllocation __joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode: __lobby.Data[key: KEY_RELAY_JOIN_CODE].Value);
                
                //Set Transform As Client (a)
                
                Transport.SetClientRelayDataFromAlloc(alloc: __joinAlloc);
                
                NetworkManager.Singleton.StartClient();

                return __lobby;
            }
            catch (Exception __exception)
            {
                Debug.Log(message: $"<color=red>[Warning] no lobbies found. ({__exception.Message})</color>");
                return null;
            }
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

        [Command(aliasOverride: "join-code")]
        public static String JoinCode { get => _joinCode; set => _joinCode = value.ToUpper(); }
        private static String _joinCode;

        [Command(aliasOverride: "leave")]
        public static void Leave() { }

        //TODO: Close server when everyone leaves.

        [Command(aliasOverride: "lobbies")]
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

        [Command(aliasOverride: "my-lobby")]
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