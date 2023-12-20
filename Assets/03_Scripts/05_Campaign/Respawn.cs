using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class Respawn : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform respawnPoint;

        void OnTriggerEnter(Collider player)
        {
            player.transform.position = respawnPoint.transform.position;
            
        }

    }
}
