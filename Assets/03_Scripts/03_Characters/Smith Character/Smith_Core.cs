using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Characters
{
    [RequireComponent(typeof(Smith_Passive))]
    [RequireComponent(typeof(Rigidbody))]
    public class Smith_Core : MonoBehaviour
    {
        // References
        private Smith_Passive Smith_Passive;
        private Rigidbody rb;


        // Variables
        [Range(0.001f, 1f)]
        [SerializeField] private float baseSpeed, baseAttackSpeed; // Base Speed, Base Attack Speed
        private float modifier; // Speed Modifier

        private float deadzone = 0f;


        private float mySpeed(float mySpeed)
        {
            if (Smith_Passive.Get_Active)
                modifier = Smith_Passive.Get_Passive_Strenght;
            else
                modifier = 0;

            mySpeed = mySpeed + modifier;
            return mySpeed;
        }

        private void Start()
        {
            Smith_Passive = this.GetComponent<Smith_Passive>();
            rb = this.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            TempMovement();

            if (Input.GetKeyDown(KeyCode.Space))
                Smith_Passive.Set_passiveFloat = 1f;
        }

        private void TempMovement()
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");

            if (hor < -deadzone) // left
            {
                transform.Translate(-Vector3.right * mySpeed(baseSpeed), Space.World);
            }
            if (hor > deadzone) // right
            {
                transform.Translate(Vector3.right * mySpeed(baseSpeed), Space.World);
            }
            if (ver < -deadzone) // backwards
            {
                transform.Translate(-Vector3.forward * mySpeed(baseSpeed), Space.World);
            }
            if (ver > deadzone) // forwards
            {
                transform.Translate(Vector3.forward * mySpeed(baseSpeed), Space.World);
            }
        }
    }
}
