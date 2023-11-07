using System.Collections;
using UnityEngine;

namespace Bonkers
{
    public class SquirrelCode : MonoBehaviour
    {
        Transform player;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;              
            StartCoroutine(Lifetime());
        }
        private void Update()
        {
            this.transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.02f);
            this.transform.LookAt(player);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Jumpscare());
            }
        }

        IEnumerator Jumpscare()
        {
            yield return new WaitForSeconds(0.2f);
            Destroy(this.gameObject);
        }
        IEnumerator Lifetime()
        {
            yield return new WaitForSeconds(0.75f);
            Destroy(this.gameObject);
        }
    }
}
