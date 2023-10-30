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

        public float ratSpeed = 10f;

        public bool enemy;

        public float lifetime = 3;

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

        public void ChaseEnemy(GameObject target)
        {
            this.gameObject.transform.position = Vector3.MoveTowards(transform.position,target.transform.position, 0.02f);
            this.gameObject.transform.LookAt(target.transform, Vector3.up);
        }
    }
}
