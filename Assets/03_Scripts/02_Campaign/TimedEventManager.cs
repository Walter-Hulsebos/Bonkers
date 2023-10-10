using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Bonkers
{
    public class TimedEventManager : CampaignSubManager
    {
        private Timer timedEventTimer;


        public override void Start()
        {
            timedEventTimer = new Timer();
            timedEventTimer.SetTimer(5);
        }


        public override void Update()
        {
            if (timedEventTimer.isActive && timedEventTimer.TimerDone())
            {
                timedEventTimer.StopTimer();
            }

            if (timedEventTimer.isActive)
            {
                CampaignManager.instance.GetTimedEventText().text = Mathf.RoundToInt(timedEventTimer.TimeLeft()).ToString();
            }
        }

        public void ReactToTimedEvent()
        {
            if (timedEventTimer.isActive && !timedEventTimer.TimerDone())
            {
                Debug.Log("You reacted on time!");
            }
            Debug.Log("Test");
        }
    }
}
