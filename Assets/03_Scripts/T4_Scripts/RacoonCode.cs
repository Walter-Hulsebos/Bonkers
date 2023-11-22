using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class RacoonCode : MonoBehaviour
    {
        Rigidbody rb;
        public GameObject bin;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (rb)
            {
                var step = 1.0f * Time.deltaTime;
                this.transform.position = Vector3.MoveTowards(this.transform.position, bin.transform.position, step);
                this.transform.LookAt(bin.transform, Vector3.up);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Bin"))
            {
                Destroy(rb);
            }
        }
    }
}
