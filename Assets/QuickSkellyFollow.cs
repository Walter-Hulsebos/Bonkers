using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bonkers
{
    public class QuickSkellyFollow : MonoBehaviour
    {
        NavMeshAgent agent;
        Transform player;
        public GameObject boo;
        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        private void Update()
        {
            agent.SetDestination(player.position);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                Instantiate(boo, transform.position, Quaternion.identity);
            }
        }
    }
}
