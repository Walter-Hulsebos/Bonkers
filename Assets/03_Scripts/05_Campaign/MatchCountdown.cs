using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class MatchCountdown : MonoBehaviour
    {
        private Timer matchCountdownTimer;

        public void Start()
        {
            matchCountdownTimer = new Timer();
        }

        public void Update()
        {
            if (matchCountdownTimer.isActive && matchCountdownTimer.TimerDone())
            {
                StopMatchCountdown();
                matchCountdownTimer.StopTimer();
            }
        }

        /// <summary>
        /// Get the remaining time of the match countdown timer.
        /// </summary>
        public float GetRemainingTime()
        {
            return matchCountdownTimer.TimeLeft();
        }

        /// <summary>
        /// Start the countdown Timer
        /// </summary>
        /// <param name="_amountInSeconds"></param>
        public void StartMatchCountdown(int _amountInSeconds)
        {
            matchCountdownTimer.SetTimer(_amountInSeconds);
        }


        /// <summary>
        /// Get the remaining time of the match countdown timer in format to use in UI. Format: 00:00
        /// </summary>
        /// <returns></returns>
        public string GetRemainingTimeFormatted()
        {
            float totalSeconds = Mathf.RoundToInt(GetRemainingTime());
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            return time.ToString("mm':'ss");
        }

        /// <summary>
        /// Get Timer Progress
        /// </summary>
        /// <returns></returns>
        public float TimerProgress()
        {
            return matchCountdownTimer.TimerProgress();
        }



        /// <summary>
        /// Method called when the timer is done.
        /// </summary>
        public void StopMatchCountdown()
        {

        }

        /// <summary>
        /// Method call for pausing the match count down timer
        /// </summary>
        /// <param name="_value"></param>
        public void PauseTimer(bool _value)
        {
            matchCountdownTimer.PauseTimer(_value);
        }

    }
}
