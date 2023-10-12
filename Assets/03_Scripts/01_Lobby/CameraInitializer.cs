namespace Bonkers.Lobby
{
    using System;

    using Unity.Cinemachine;
    using Unity.Netcode;

    using UnityEngine;
    
    using JetBrains.Annotations;
    
    using U64 = System.UInt64;

    [PublicAPI]
    public sealed class CameraInitializer : NetworkBehaviour
    {
        [SerializeField] private CinemachineBrain  cinemachineBrain;
        [SerializeField] private CinemachineCamera cinemachineCamera;

        #if UNITY_EDITOR
        private void Reset()
        {
            cinemachineBrain  = GetComponentInChildren<CinemachineBrain>();
            cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        }
        #endif

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            ConfigureCameraChannelsRemotePlayers();
        }

        private void ConfigureCameraChannelsRemotePlayers()
        {
            //ulong __ownerClientId = NetworkManager.Singleton.LocalClientId;

            U64 __ownerClientId = OwnerClientId;
            
            Debug.Log($"Owner client ID: {__ownerClientId}");
            
            // Get owner index
            
            
            
            // Shift one bit per player count.
            // OutputChannels __outputChannels = (OutputChannels)(1 << __remoteClients);
            //
            // Debug.Log($"Initializing cameras for {__remoteClients} players, shifting to {__outputChannels} output channel(s).");
            //
            // cinemachineBrain.ChannelMask    = __outputChannels;
            // cinemachineCamera.OutputChannel = __outputChannels;
        }

        public void ConfigureCameraChannelsLocalPlayers()
        {
            // Shift one bit per player count.
            OutputChannels __outputChannels = (OutputChannels)(1 << Players.LocalCount);
            
            Debug.Log($"Initializing cameras for {Players.LocalCount} players, shifting to {__outputChannels} output channel(s).");
            
            cinemachineBrain.ChannelMask    = __outputChannels;
            cinemachineCamera.OutputChannel = __outputChannels;
        }
    }
}