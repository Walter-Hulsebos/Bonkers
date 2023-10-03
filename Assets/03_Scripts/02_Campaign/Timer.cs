using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Bonkers
{
    public class Timer
    {
        public float timeStamp;
        public float interval;
        public float pauseDifference;

        public bool isPaused { get; private set; }
        public bool isActive { get; private set; }


        public float TimeLeft()
        {

            return TimerDone() ? 0 : (1 - TimerProgress()) * interval;
        }

        public void SetTimer(float _interval = 2)
        {
            timeStamp = Time.time;
            interval = _interval;
            isActive = true;
        }


        public float TimerProgress() 
        {
            return (isPaused) ? (interval - pauseDifference / interval) : TimerDone() == true ? 1 : Mathf.Abs((timeStamp - Time.time) / interval);
        }

        public void RestartTimer()
        {
            SetTimer(interval);
        }


        public void StopTimer()
        {
            isActive = false;
            timeStamp = interval;
        }

        public bool TimerDone()
        {
            return (isPaused) ? pauseDifference == 0.0f : Time.deltaTime >= timeStamp;
        }

        public void PauseTimer(bool pause)
        {
            if (pause)
            {
                pauseDifference = TimeLeft();
                isPaused = pause;
                return;
            }
            isPaused = pause;
            timeStamp = Time.time - (interval - pauseDifference);
        }
    }
}
