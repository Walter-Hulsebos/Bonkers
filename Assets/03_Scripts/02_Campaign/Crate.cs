using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class Crate : MonoBehaviour
    {
        public void ReceiveHit() 
        {
            print("You hit a crate");
            Destroy(gameObject);
        }
    }
}
