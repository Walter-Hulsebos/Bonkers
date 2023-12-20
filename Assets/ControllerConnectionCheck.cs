using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bonkers
{
    public class ControllerConnectionCheck : MonoBehaviour
    {
        [SerializeField] private GameObject[] gameObjects;

        private bool connected;

        void FixedUpdate()
        {
            var gamepad = Gamepad.current;
            if (gamepad == null)
            {
                Debug.Log("Disconnected");
                connected = false;
            }
            else
            {
                connected = true;
                Debug.Log("Connected");
            }
        }

        private void Update()
        {
            if (connected)
            {
                foreach (var gameObject in gameObjects)
                {
                    gameObject.SetActive(false);
                }
            }
            if (!connected)
            {
                foreach (var gameObject in gameObjects)
                {
                    gameObject.SetActive(true);
                }
            }
        }
    }
}
