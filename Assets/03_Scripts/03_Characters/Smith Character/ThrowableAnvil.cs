using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Bonkers.Characters
{


    public class ThrowableAnvil : MonoBehaviour
    {
        public Rigidbody anvil;
        public float throwforce = 10;
        public Transform target, curve_point;
        private Vector3 old_pos;
        private bool isReturning = false;
        private float time = 0.0f;
        public bool aiming = false;
        private bool isThrown = false;
        private Vector3 initialPosition;

        public Smith_Passive smithPassive;
        public float originalReturnDelay = 5f;
        public float fasterReturnMultiplier = 2f;



        // Update is called once per frame
        void Update()
        {

            if (Input.GetButtonUp("Fire1") && !isThrown)
            {
                ThrowAnvil();
            }


            if (isReturning)
            {
                //returning calculations
                if (time < 1.0f)
                {
                    anvil.position = getBQCPoint(time, old_pos, curve_point.position, target.position);
                    anvil.rotation = Quaternion.Slerp(anvil.transform.rotation, target.rotation, 50 * Time.deltaTime);
                    time += Time.deltaTime;
                }
                else
                {
                    ResetAnvil();

                }

            }
        }

        // Throw Anvil
        void ThrowAnvil()
        {
            initialPosition = anvil.position;
            isReturning = false;
            anvil.transform.parent = null;
            anvil.isKinematic = false;
            anvil.AddForce(Camera.main.transform.TransformDirection(Vector3.forward) * throwforce, ForceMode.Impulse);
            anvil.AddTorque(anvil.transform.TransformDirection(Vector3.forward) * 100, ForceMode.Impulse);
            isThrown = true;

            // Trigger passive ability when the anvil is thrown
            if (smithPassive != null)
            {
                smithPassive.Set_passiveFloat = 1f;
            }

            // Start coroutine to return the anvil after a delay
            StartCoroutine(ReturnAnvilAfterDelay(5f)); ;
        }
        // Return Anvil
        void ReturnAnvil()
        {
            time = 0.0f;
            old_pos = anvil.position;
            isReturning = true;
            anvil.velocity = Vector3.zero;
            anvil.isKinematic = true;

        }
        // Reset Hammer
        void ResetAnvil()
        {
            isReturning = false;
            anvil.transform.parent = transform;
            anvil.position = target.position;
            anvil.rotation = target.rotation;
        }
        Vector3 getBQCPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
            return p;
        }
        IEnumerator ReturnAnvilAfterDelay(float delay)
        {
            float returnDelay = smithPassive.Get_Active ? originalReturnDelay / fasterReturnMultiplier : originalReturnDelay;

            yield return new WaitForSeconds(returnDelay);

            // Reset hammer properties
            anvil.velocity = Vector3.zero;
            anvil.angularVelocity = Vector3.zero;
            anvil.isKinematic = true;

            // Move the hammer back to its initial position
            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * (smithPassive.Get_Active ? fasterReturnMultiplier : 1f);
                anvil.transform.position = Vector3.Lerp(anvil.transform.position, initialPosition, elapsedTime);
                yield return null;
            }

            isThrown = false; // Hammer has returned to the initial position
            anvil.transform.parent = transform; // Reattach the hammer to the player
        }
    }
}
