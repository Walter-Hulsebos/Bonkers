using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Bonkers
{
    public class Powerup : MonoBehaviour
    {
        public void OnTriggerEnter(Collider collider)

        {
            if (collider.gameObject.CompareTag("Player"))
            {
                //Handle powerup
                Debug.Log("Powerup picked up");

                var stateMachineRef = collider.gameObject.GetComponentInChildren<PlayerStateMachine>();
                int multiplier = 2;
                float playerOriginalSpeed = stateMachineRef.Data.PlayerMaxSpeed;
                var playerMultipliedSpeed = playerOriginalSpeed * multiplier;

                stateMachineRef.Data.PlayerMaxSpeed = playerMultipliedSpeed;
                StartCoroutine(waiter());
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
                gameObject.GetComponent<MeshRenderer>().enabled = false;

                IEnumerator waiter()
                {
                    Debug.Log("I am waiting");
                    yield return new WaitForSeconds(5);
                    Debug.Log("stopped waiting");
                    stateMachineRef.Data.PlayerMaxSpeed = playerOriginalSpeed;
                    Debug.Log(stateMachineRef.Data.PlayerMaxSpeed);
                    Destroy(gameObject);
                }
            }
        }
    }
}


