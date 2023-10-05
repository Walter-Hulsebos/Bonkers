using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Bonkers
{
    public class TimedEventManager : MonoBehaviour
    {
        private Timer timedEventTimer;
        [SerializeField] TextMeshProUGUI timedEvent;
     
        private void Start()
        {
            timedEventTimer = new Timer();
            timedEventTimer.SetTimer(5);
        }

        

        public void Update()
        {
            if (timedEventTimer.isActive && timedEventTimer.TimerDone())
            {
                timedEventTimer.StopTimer();
            }

            if (timedEventTimer.isActive)
            {
                timedEvent.text = Mathf.RoundToInt(timedEventTimer.TimeLeft()).ToString();
            }
        }

        public void ReactToTimedEvent(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (timedEventTimer.isActive && !timedEventTimer.TimerDone())
                {
                    Debug.Log("You reacted on time!");
                }
                Debug.Log("Test");
            }
        }



    }
}
