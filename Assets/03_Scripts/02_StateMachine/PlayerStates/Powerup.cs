using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class Powerup : MonoBehaviour
    {
        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                //Handle powerup
                Debug.Log("Powerup picked up");
                Destroy(this.gameObject);
            }
        }
    }
}
