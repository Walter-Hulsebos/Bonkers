using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class Elevator : MonoBehaviour
    {
     [SerializeField] public float speed = 2.0f;
     [SerializeField]public float maxHeight = 10.0f;

    void Update()
    {
        if (transform.position.y < maxHeight)
        {
            Vector3 movement = new Vector3(0.0f, 0.0f, 1.0f);
            transform.Translate(movement * speed * Time.deltaTime);
        }
    }
    }
}
