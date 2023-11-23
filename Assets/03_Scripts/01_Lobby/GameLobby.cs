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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Debug.Log("Resetting GameLobby static variables to defaults.");

            _heartbeatTimer = 0f;
            _joinedLobby    = null;

            //Unsubscribe all listeners
            OnLobbyListChanged = null;
        }

        private const  String                   KEY_RELAY_JOIN_CODE = "Rughaar";
        private static F32                      _heartbeatTimer;
        private static Lobby                    _joinedLobby;
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
                if (HasTransport) { return _cachedTransport; }

                if (HasManager)
                {
                    _cachedTransport = Manager.GetComponent<UnityTransport>();

                    if (_cachedTransport == null) { Debug.LogError("No UnityTransport on the NetworkManager!"); }
                }
                else { Debug.LogError("No NetworkManager in the scene!"); }

                return _cachedTransport;
            }
        }

        /*
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void AfterSceneLoad()
        {
            await Authenticate();
        }
        */

        public static async UniTask Authenticate()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized) { return; }

            InitializationOptions __initializationOptions = new();

            #if UNITY_EDITOR
            Bool __isClone = ClonesManager.IsClone();

            __initializationOptions.SetProfile(__isClone ? ClonesManager.GetArgument() : "Primary");
            #endif

            Debug.Log("Initializing Unity Services...");
            await UnityServices.InitializeAsync(__initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
        }

        [Command("host")]
        public static async void Host(String name, I32 teamSizes = 3)
        {
            try
            {
                I32 __maxConnections = teamSizes * 2;

                Allocation __hostAlloc = await RelayService.Instance.CreateAllocationAsync(__maxConnections);
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(__hostAlloc.AllocationId);

                CreateLobbyOptions __options = new()
                {
                    Data = new Dictionary<String, DataObject> { { KEY_RELAY_JOIN_CODE, new DataObject(Public, _joinCode) }, },
                };

                Lobby __lobby = await LobbyService.Instance.CreateLobbyAsync(name, __maxConnections, __options);

                // Start a heartbeat timer to keep the lobby alive. 

                Transport.SetHostRelayDataFromAlloc(__hostAlloc);

                if (NetworkManager.Singleton.StartHost())
                {
                    Debug.Log
                    (
                        $"<color=#00ff00ff>Hosting lobby {name} with join code </color><b>{JoinCode}</b><color=#00ff00ff>, max connections: {__maxConnections}</color>"
                    );
                }
                else { Debug.LogError("Failed to start host!"); }
            }
            catch (LobbyServiceException __serviceException) { Debug.Log(__serviceException); }
        }

        [Command("join")]
        public static async void Join(String joinCode)
        {
            JoinAllocation __joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

            Transport.SetClientRelayDataFromAlloc(__joinAlloc);

            NetworkManager.Singleton.StartClient();
        }

        [Command("quick-join")]
        public static async UniTask<Lobby> QuickJoin()
        {
            try
            {
                // Attempt to join a lobby in progress
                QuickJoinLobbyOptions __quickJoinOptions = new()
                {
                    Filter = new List<QueryFilter>
                    {
                        new (QueryFilter.FieldOptions.AvailableSlots, "0",     QueryFilter.OpOptions.GT),
                        new (QueryFilter.FieldOptions.IsLocked,       "false", QueryFilter.OpOptions.EQ),
                    },
                };

                Lobby __lobby = await LobbyService.Instance.QuickJoinLobbyAsync(__quickJoinOptions);

                if (__lobby == null)
                {
                    Debug.Log($"No lobbies found. Retrying in 3 seconds...");
                    await UniTask.Delay(1000);
                    Debug.Log("2");
                    await UniTask.Delay(1000);
                    Debug.Log("1");
                }

                __lobby = await LobbyService.Instance.QuickJoinLobbyAsync(__quickJoinOptions);

                if (__lobby == null) { Debug.LogWarning($"No lobbies found after second atempt, <b>aborting</b>!"); }


                // If we found one grab the relay allocation details.
                JoinAllocation __joinAlloc = await RelayService.Instance.JoinAllocationAsync(__lobby.Data[KEY_RELAY_JOIN_CODE].Value);

                //Set Transform As Client (a)

                Transport.SetClientRelayDataFromAlloc(__joinAlloc);

                NetworkManager.Singleton.StartClient();

                return __lobby;
            }
            catch (Exception __exception)
            {
                Debug.Log($"<color=red>[Warning] no lobbies found. ({__exception.Message})</color>");
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

        [Command("join-code")]
        public static String JoinCode
        {
            get => _joinCode;
            set => _joinCode = value.ToUpper();
        }

        private static String _joinCode;

        [Command("leave")]
        public static void Leave() { }

        //TODO: Close server when everyone leaves.

        [Command("lobbies")]
        public static async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions __queryLobbiesOptions = new()
                {
                    Filters = new List<QueryFilter> { new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT), },
                };

                QueryResponse __queryResponse = await LobbyService.Instance.QueryLobbiesAsync(__queryLobbiesOptions);

                OnLobbyListChanged?.Invoke(__queryResponse.Results);

                if (__queryResponse.Results == null || __queryResponse.Results.Count == 0)
                {
                    Debug.Log("No lobbies found!");
                    return;
                }

                StringBuilder __stringBuilder = new();

                foreach (Lobby __lobby in __queryResponse.Results)
                {
                    __stringBuilder.AppendLine($"Lobby: {__lobby.Id} - [{__lobby.AvailableSlots}/{__lobby.MaxPlayers}]");
                }

                Debug.Log(__stringBuilder.ToString());
            }
            catch (LobbyServiceException __e) { Debug.Log(__e); }
        }

        [Command("my-lobby")]
        public static Lobby MyLobby() => _joinedLobby;

        private static void HandleHeartbeat()
        {
            if (!IsLobbyHost) { return; }

            _heartbeatTimer -= Time.deltaTime;

            if (_heartbeatTimer > 0f) { return; }

            const F32 HEARTBEAT_TIMER_MAX = 15f;
            _heartbeatTimer = HEARTBEAT_TIMER_MAX;

            LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
        }

        private static Boolean IsLobbyHost => _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
}