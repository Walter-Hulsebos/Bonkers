using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class Knockbacktests : MonoBehaviour
    {
 [SerializeField] private float launchForce = 50;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player entered box");

                if (other.TryGetComponent(out PlayerStateMachine __stateMachine))
                {
                    LaunchPlayer(__stateMachine);
                }
            }
            
        }

        private void LaunchPlayer(PlayerStateMachine stateMachine)
        {
          stateMachine.Motor.ForceUnground(time: 0.1f);
          Vector3 launchDirection = transform.rotation * Vector3.forward;
          Vector3 launchForceVector = Vector3.up * launchForce + launchDirection * launchForce;
          stateMachine.AddVelocity(launchForceVector);
        }
    }
}
