using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class WindZoneHigh : MonoBehaviour
    {
        List<Rigidbody> RigidbodiesInWindZoneList = new List<Rigidbody>();
        Vector3 windDirection = Vector3.left;
        [SerializeField]
        float windStrength = 5;
        
        private void OnTriggerEnter(Collider col){
            Rigidbody objectRigid = col.gameObject.GetComponent<Rigidbody>();
            if(objectRigid != null)
                RigidbodiesInWindZoneList.Add(objectRigid);
        }

        private void OnTriggerExit(Collider col){
            Rigidbody objectRigid = col.gameObject.GetComponent<Rigidbody>();
            if(objectRigid != null)
                RigidbodiesInWindZoneList.Remove(objectRigid);
        }

        private void FixedUpdate(){
            if(RigidbodiesInWindZoneList.Count > 0) {
                foreach (Rigidbody rigid in RigidbodiesInWindZoneList)
                {
                    rigid.AddForce(windDirection * windStrength);
                }
            }
        }
    }
}
