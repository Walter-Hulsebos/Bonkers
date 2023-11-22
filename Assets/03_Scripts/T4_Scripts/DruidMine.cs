using System.Collections;
using UnityEngine;

namespace Bonkers
{
    public class DruidMine : MonoBehaviour
    {
        public GameObject squirrel;
        Animator animator;
        private void Awake()
        {
            animator = GetComponent<Animator>();
            animator.Play("MineComingIn");
        }
        //should be on enter but put it to stay for the skelly jumpscare
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(GoCrazy());
            }
        }

        IEnumerator GoCrazy()
        {
            Instantiate(squirrel, transform.position, Quaternion.identity);
            StopAllCoroutines();
            Destroy(gameObject);
            yield return null;
        }
    }
}
