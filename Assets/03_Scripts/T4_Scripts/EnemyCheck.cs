using CGTK.Utils.UnityFunc;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Bonkers
{
    public class EnemyCheck : MonoBehaviour
    {
        RatCode ratCode;
        private void Awake()
        {
            ratCode = GetComponentInParent<RatCode>();
            StartCoroutine(Lifetime());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                ratCode.enemy = true;
                ratCode.SavedEnemy(other.gameObject);
            }
        }

        IEnumerator Lifetime()
        {
            yield return new WaitForSeconds(9999f);
            Destroy(this.gameObject);
        }
    }
}
