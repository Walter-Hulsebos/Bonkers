using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class CampaignPlayer : MonoBehaviour
    {
        [SerializeField] private float knockbackPercentage;
        private Rigidbody rb;
        // Start is called before the first frame update

        
        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody>();
            CampaignManager.instance.player = gameObject.transform.parent.gameObject;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public float GetCurrentKnockBack()
        {
            return knockbackPercentage;
        }

        public void SetKnockBackPercentage(float amount, Vector3 fromObject)
        {

            float calcValue = amount * 0.1f;
            knockbackPercentage += calcValue;

            if (rb != null)
            {
                Vector3 direction = fromObject - gameObject.transform.position;
                rb.AddForce(direction.normalized * knockbackPercentage, ForceMode.Impulse);
            }
        }
    }
}
public enum Operator
{
    ADD,
    SUBSTRACT
}