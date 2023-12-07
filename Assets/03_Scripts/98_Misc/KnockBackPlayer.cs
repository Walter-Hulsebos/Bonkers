using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class KnockBackPlayer : MonoBehaviour
    {
        [SerializeField] private float launchForce = 50;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Knockback activated");

                if (other.TryGetComponent(out PlayerStateMachine __stateMachine))
                {
                    KnockbackPlayer(__stateMachine);
                }
            }
            
        }

        private void KnockbackPlayer(PlayerStateMachine stateMachine)
        {
            stateMachine.Motor.ForceUnground(time: 0.1f);
            stateMachine.AddVelocity(Vector3.up + Vector3.back * launchForce);
        }
    }
}
