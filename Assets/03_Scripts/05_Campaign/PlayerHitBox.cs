using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bonkers
{
    public class PlayerHitBox : MonoBehaviour
    {
        private GameObject hitObject;
        private bool canTrigger;

        private void Awake()
        {

        }
        private void Start()
        {

        }

        private void OnTriggerStay(Collider collider)
        {
            switch (collider.transform.tag)
            {
                case "Enemy":
                case "Crate":
                default:
                    hitObject = collider.gameObject;
                    canTrigger = true;
                   
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            hitObject = null;
            canTrigger = false;
           
        }

        public void OnReceiveInput()
        {
            if (canTrigger)
            {
                if (hitObject != null)
                {
                   
                    switch (hitObject.tag)
                    {
                        case "Enemy":
                            hitObject.transform.GetComponent<Enemy>().TakeDamage(1f);
                            break;
                        case "Crate":
                            hitObject.transform.GetComponent<Crate>().ReceiveHit();
                            break;
                        case "Heirloom":
                        default:
                            break;
                    }
                }

            }

        }
    }
}
