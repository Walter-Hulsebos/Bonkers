using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class PigeonQuick : MonoBehaviour
    {
        GameObject targ;

        private void Awake()
        {
            targ = GameObject.FindGameObjectWithTag("Player");
        }
        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, targ.transform.position, 0.03f);
        }
    }
}
