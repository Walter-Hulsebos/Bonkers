using System;
using UnityEngine;
using TMPro;

namespace Bonkers
{
    public class MatchCountdown : MonoBehaviour
    {
        private Timer matchCountdownTimer;
        private Timer refreshTimer;

        [SerializeField] private float refreshRate;
        [SerializeField] private TextMeshProUGUI matchText;

        [Tooltip("Time in seconds")]
        [SerializeField] private float amountOfTime;
        /// <summary>
        /// Called when scene started
        /// </summary>
        public void Start()
        {
            refreshTimer = new Timer();
            matchCountdownTimer = new Timer();
            StartMatchCountdown();
            refreshTimer.SetTimer(refreshRate);
        }
        /// <summary>
        /// Called every frame
        /// </summary>
        public void Update()
        {
            if (matchCountdownTimer.isActive && matchCountdownTimer.TimerDone())
            {
                StopMatchCountdown();
                matchCountdownTimer.StopTimer();
            }

            if (refreshTimer.isActive && refreshTimer.TimerDone())
            {
                RefreshUI();
                refreshTimer.StopTimer();
                refreshTimer.SetTimer(refreshRate);
            }
        }
        /// <summary>
        /// Refresh UI with a refresh rate instead of updating it every frame
        /// </summary>
        public void RefreshUI()
        {
            matchText.text = GetRemainingTimeFormatted();   
        }

        /// <summary>
        /// Get the remaining time of the match countdown timer in raw seconds.
        /// Please dont call this in Update (Rather every second or half a second)
        /// </summary>
        public float GetRemainingTime()
        {
            return matchCountdownTimer.TimeLeft();
        }

        /// <summary>
        /// Start the countdown Timer
        /// </summary>
        /// <param name="_amountInSeconds"></param>
        public void StartMatchCountdown()
        {
            matchCountdownTimer.SetTimer(amountOfTime);
        }


        /// <summary>
        /// Get the remaining time of the match countdown timer in format to use in UI. Format: 00:00
        /// Please dont call this in Update (Rather every second or half a second)
        /// </summary>
        /// <returns></returns>
        public string GetRemainingTimeFormatted()
        {
            float totalSeconds = Mathf.RoundToInt(GetRemainingTime());
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            return time.ToString("mm':'ss");
        }

        /// <summary>
        /// Get Timer Progress.
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
            //Add Connection here
        }

        /// <summary>
        /// Method call for pausing the match count down timer.
        /// </summary>
        /// <param name="_value"></param>
        public void PauseTimer(bool _value)
        {
            matchCountdownTimer.PauseTimer(_value);
        }

    }
}
