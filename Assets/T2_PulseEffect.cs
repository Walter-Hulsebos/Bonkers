using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class T2_PulseEffect : MonoBehaviour
    {
       [SerializeField] public float pulseSpeed = 1.0f;
       [SerializeField] public float pulseScale = 0.1f;

        private void Update()
        {
            // Calculate the pulsing scale using a sine function
            float scale = 1.0f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;

            // Apply the scale to the object's transform
            transform.localScale = new Vector3(scale, scale, 1.0f);
        }
    }
}
