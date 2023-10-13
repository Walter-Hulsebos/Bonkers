using CGTK.Utils.UnityFunc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class RatCode : MonoBehaviour
    {

        GameObject player;
        Rigidbody rb;
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            rb = GetComponent<Rigidbody>();
            transform.rotation = player.transform.rotation;
            StartCoroutine(Lifetime());
        }
        void Update()
        {
            rb.velocity = transform.forward * 3.5f; 
        }

        IEnumerator Lifetime()
        {
            yield return new WaitForSeconds(4);
            Destroy(gameObject);
        }
    }
}
