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
        Vector3 backwardForce = -stateMachine.transform.forward * launchForce;
        Vector3 launchForceVector = Vector3.up * launchForce + backwardForce;

       // Vector3 launchDirection = (stateMachine.transform.position - launchOrigin).normalized;
       // Vector3 launchForceVector = launchDirection * launchForce;
        stateMachine.AddVelocity(launchForceVector);
        }
    }
}
