using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class BreakableWall : MonoBehaviour
    {
        [SerializeField] private float breakTreshold;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                KinematicCharacterController.KinematicCharacterMotor kinematicCharacterMotor = 
                    other.GetComponent<KinematicCharacterController.KinematicCharacterMotor>();

                if (kinematicCharacterMotor.Velocity.magnitude >= breakTreshold)
                {
                    BreakWall();
                }
            }
        }

        private void BreakWall()
        {
            Destroy(gameObject);
        }
    }
}
