using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour {

    public Animation hingehere;
    public GameObject keygone;
    public GameObject keygone1;
    public GameObject keygone2;
    private bool istriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider Other)
    {
        if (istriggered == false)
        {
            if (keygone.activeSelf == false && keygone1.activeSelf == false && keygone2.activeSelf == false)
            {
                hingehere.Play();
                istriggered = true;
            }

            void OnTriggerExit(Collider coll)
            {
                istriggered = false;
            }
        }
    }    
    }
