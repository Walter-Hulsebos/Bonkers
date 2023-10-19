using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class shatter : MonoBehaviour
    {
         private Rigidbody rb;
    private bool gravityActivated = false;

    // Adjustable delay time in seconds
   [SerializeField] private float delay = 10f; // Time delay before activating gravity

    // Blink parameters
    private float blinkInterval = 0.5f; // Blink interval in seconds
    private float nextBlinkTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Initialize next blink time
        nextBlinkTime = delay - blinkInterval;
    }

    void Update()
    {
        // Check if gravity is not activated and the delay has passed
        if (!gravityActivated && Time.time >= delay)
        {
            // Activate gravity
            rb.useGravity = true;
            gravityActivated = true;

        }
        else
        {
            // Check if it's time to blink
            if (!gravityActivated && Time.time >= nextBlinkTime)
            {
                // Toggle the object's color between red and its original color
                GetComponent<Renderer>().material.color = Color.red;
                Invoke("ResetColor", blinkInterval / 2f);

                // Calculate the next blink time
                nextBlinkTime = Time.time + blinkInterval;
            }
        }
    }

    // Reset the object's color to its original color
    void ResetColor()
    {
        GetComponent<Renderer>().material.color = Color.white; // Change to the original color
    }
    }
}

