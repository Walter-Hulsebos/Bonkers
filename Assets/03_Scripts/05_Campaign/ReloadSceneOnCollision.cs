using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KinematicCharacterController;

namespace Bonkers
{
    public class ReloadSceneOnCollision : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        
        private void OnTriggerEnter(Collider other)
        {
            if (respawnPoint != null)
            {
                if (other.CompareTag("Player"))
                {
                    other.GetComponent<KinematicCharacterMotor>().MoveCharacter(respawnPoint.transform.position);
                }
            }
        }
    }
}
