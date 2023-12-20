using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class RespawnManager : MonoBehaviour
    {
        public int maxRespawns;

        void Start()
        {

            PlayerRespawn[] playerRespawns = FindObjectsOfType<PlayerRespawn>();

            foreach (PlayerRespawn respawnScript in playerRespawns)
            {
                respawnScript.maxRespawns = maxRespawns;
            }
        }
    }
}
