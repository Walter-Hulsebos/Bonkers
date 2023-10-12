using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class SwapObject : MonoBehaviour
    {
        public GameObject destroyedVersion;

        void OnMouseDown ()
        {
            Instantiate(destroyedVersion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
