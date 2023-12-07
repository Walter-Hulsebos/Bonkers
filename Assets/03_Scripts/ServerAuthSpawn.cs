namespace Bonkers
{
    using System;
    using System.Collections.Generic;

    using Unity.Cinemachine;
    using Unity.Netcode;
    using Unity.Netcode.Components;
    
    using Bonkers.Shared;

    using KinematicCharacterController;
    
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    using UnityEngine;
    
    using I32  = System.Int32;
    using Bool = System.Boolean;
    using Cysharp.Threading.Tasks;

    /// <summary>
    /// Server side script to do some movements that can only be done server side with Netcode.
    /// In charge of spawning (which happens server side in Netcode)
    /// </summary>
    [DefaultExecutionOrder(0)] // before client component
    public sealed class ServerAuthSpawn : NetworkBehaviour
    {
        [SerializeField] private KinematicCharacterMotor _motor;
        [SerializeField] private PlayerTeam              _team;
        [SerializeField] private NetworkTransformClientAuthoritative _transformClientAuth;
        #if ODIN_INSPECTOR
        [ChildGameObjectsOnly]
        #endif
        [SerializeField] private Transform               _cameraTarget;

        private void Reset()
        {
            _motor        = GetComponentInChildren<KinematicCharacterMotor>();
            _team         = GetComponentInChildren<PlayerTeam>();
            _cameraTarget = transform.FindChildWithTag("CameraTarget");
            _transformClientAuth = GetComponentInChildren<NetworkTransformClientAuthoritative>();
        }

        public override void OnNetworkSpawn()
        {
/*            if (!IsServer)
            {
                enabled = false;
                return;
            }*/

            OnServerSpawnPlayer();

            base.OnNetworkSpawn();
        }

        private void OnServerSpawnPlayer()
        {
            // this is done server side, so we have a single source of truth for our spawn point list

            SpawnHelpers.SpawnPlayer(team: _team, motor: _motor, cameraTarget: _cameraTarget);
            Debug.Log("spawned player", context: this);

            // A note specific to owner authority:
            // Side Note:  Specific to Owner Authoritative
            // Setting the position works as and can be set in OnNetworkSpawn server-side unless there is a
            // CharacterController that is enabled by default on the authoritative side. With CharacterController, it
            // needs to be disabled by default (i.e. in Awake), the server applies the position (OnNetworkSpawn), and then
            // the owner of the NetworkObject should enable CharacterController during OnNetworkSpawn. Otherwise,
            // CharacterController will initialize itself with the initial position (before synchronization) and updates the
            // transform after synchronization with the initial position, thus overwriting the synchronized position.
        }



        
        
        
    }
}