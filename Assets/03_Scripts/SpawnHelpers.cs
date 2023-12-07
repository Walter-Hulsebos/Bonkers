using Bonkers.Shared;
using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Cinemachine;
using UnityEngine;

using I32 = System.Int32;

namespace Bonkers
{
    public static class SpawnHelpers
    {
        //[PublicAPI]
        public static void SpawnPlayer(Team team, KinematicCharacterMotor motor, Transform cameraTarget)
        {
            //await UniTask.Yield();
            Vector3 __spawnPoint = RandomSpawnPoint(team);
            Vector3 __delta      = (cameraTarget.position - __spawnPoint);

            //TODO make it not interpolate to the spawn position

            motor.SetPosition(position: __spawnPoint, bypassInterpolation: true);
            CinemachineCore.OnTargetObjectWarped(target: cameraTarget, positionDelta: __delta);
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

            StringBuilder __stringBuilder = new StringBuilder();
            __stringBuilder.Append("<b> All Spawnpoints: </b> \n");

            List<Spawn> __spawnPointsForTeam = new();

            foreach (Spawn __spawnPoint in __allSpawnPoints)
            {
                __stringBuilder.Append(__spawnPoint.name + ", \n");

                if (__spawnPoint.Team == team)
                {
                    __spawnPointsForTeam.Add(__spawnPoint);
                }
            }
            Debug.Log(__stringBuilder.ToString());
            

            if (__spawnPointsForTeam.Count == 0)
            {
                Debug.LogError($"No spawn points for team {team}");
                return null;
            }

            return __spawnPointsForTeam;
        }
    }
}
