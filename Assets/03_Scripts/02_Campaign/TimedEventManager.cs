using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bonkers
{
    public class TimedEventManager : MonoBehaviour
    {
        private Timer timedEventTimer;

        TimedEventManager()
        {
            timedEventTimer = new Timer();
        }

        public void StartTimedEvent(float interval = 5)
        {
            timedEventTimer.SetTimer(interval);
        }

        public void Update()
        {

        }

        public void ReactToTimedEvent(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("Test");
            }
        }



    }
}
