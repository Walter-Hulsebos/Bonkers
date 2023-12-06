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

    /// <summary>
    /// Server side script to do some movements that can only be done server side with Netcode.
    /// In charge of spawning (which happens server side in Netcode)
    /// </summary>
    [DefaultExecutionOrder(0)] // before client component
    public sealed class CharacterSpawner : NetworkBehaviour
    {
        [SerializeField] private KinematicCharacterMotor _motor;
        [SerializeField] private PlayerTeam              _team;
        #if ODIN_INSPECTOR
        [ChildGameObjectsOnly]
        #endif
        [SerializeField] private Transform               _cameraTarget;

        private void Reset()
        {
            _motor        = GetComponent<KinematicCharacterMotor>();
            _team         = GetComponent<PlayerTeam>();
            _cameraTarget = transform.Find("CharacterCameraTarget").transform;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }

            OnServerSpawnPlayer();

            base.OnNetworkSpawn();
        }

        private void OnServerSpawnPlayer()
        {
            // this is done server side, so we have a single source of truth for our spawn point list
            _motor.SetPosition(position: transform.position, bypassInterpolation: true);
            
            // A note specific to owner authority:
            // Side Note:  Specific to Owner Authoritative
            // Setting the position works as and can be set in OnNetworkSpawn server-side unless there is a
            // CharacterController that is enabled by default on the authoritative side. With CharacterController, it
            // needs to be disabled by default (i.e. in Awake), the server applies the position (OnNetworkSpawn), and then
            // the owner of the NetworkObject should enable CharacterController during OnNetworkSpawn. Otherwise,
            // CharacterController will initialize itself with the initial position (before synchronization) and updates the
            // transform after synchronization with the initial position, thus overwriting the synchronized position.
        }

        private void OnSpawnPlayer()
        {
            Vector3 __spawnPoint = RandomSpawnPoint(_team.Team);
            Vector3 __delta      = (transform.position - __spawnPoint);
            
            
            
            _motor.SetPosition(position: __spawnPoint, bypassInterpolation: true);
            CinemachineCore.OnTargetObjectWarped(target: transform, positionDelta: __delta);
        }

        private static Vector3 RandomSpawnPoint(Team team) => RandomSpawn(team: team).RandomPointOnSpawnCircle();

        private static Spawn RandomSpawn(Team team)
        {
            List<Spawn> __spawnPointsForTeam = SpawnsForTeam(team);

            if (__spawnPointsForTeam.Count == 0)
            {
                Debug.LogError($"No spawn points for team {team}");
                return null;
            }

            I32 __randomIndex = UnityEngine.Random.Range(0, __spawnPointsForTeam.Count);
            
            //TODO: Do Check if spawn point is occupied first, otherwise, repeat function.

            return __spawnPointsForTeam[__randomIndex];
        }
        
        private static List<Spawn> SpawnsForTeam(Team team)
        {
            List<Spawn> __allSpawnPoints = Spawn.Instances;
            
            List<Spawn> __spawnPointsForTeam = new();
            
            foreach (Spawn __spawnPoint in __allSpawnPoints)
            {
                if (__spawnPoint.Team == team)
                {
                    __spawnPointsForTeam.Add(__spawnPoint);
                }
            }
            
            if (__spawnPointsForTeam.Count == 0)
            {
                Debug.LogError($"No spawn points for team {team}");
                return null;
            }

            return __spawnPointsForTeam;
        }
        
        
    }
}