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

        public bool enemy;

        public float lifetime = 3;
        public float ratSpeed = 10;

        public GameObject savedEnemy;
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            rb = GetComponent<Rigidbody>();
            transform.rotation = player.transform.rotation;
            StartCoroutine(Lifetime());
        }
        void Update()
        {
            if (enemy == false)
                rb.velocity = transform.forward * ratSpeed;
            else
            if (enemy == true)
                ChaseEnemy(savedEnemy);
        }

        IEnumerator Lifetime()
        {
            yield return new WaitForSeconds(lifetime);
            Destroy(this.gameObject);
        }

        public void SavedEnemy(GameObject gameObject)
        {
            savedEnemy = gameObject;
        }

        public void ChaseEnemy(GameObject gameObject)
        {
            this.gameObject.transform.position = Vector3.MoveTowards(transform.position, gameObject.transform.position, 0.02f);
            this.gameObject.transform.LookAt(gameObject.transform, Vector3.up);
        }
    }
}
