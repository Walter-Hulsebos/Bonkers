using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float timeStamp;
    private float interval;
    private float pauseDifference;

    public bool isPaused { get; private set; }
    public bool isActive { get; private set; }

    /// <summary>
    /// Method call for checking how much time is left on the timer
    /// </summary>
    /// <returns></returns>
    public float TimeLeft()
    {
        return TimerDone() ? 0 : (1 - TimerProgress()) * interval;
    }

    /// <summary>
    /// Method call for checking how much time has elapsed on the timer
    /// </summary>
    /// <returns></returns>
    public float TimerProgress()
    {
        return (isPaused) ? (interval - pauseDifference / interval) : TimerDone() == true ? 1 : Mathf.Abs((timeStamp - Time.time) / interval);
    }

    /// <summary>
    /// Method call for checking if the timer has been completed.
    /// </summary>
    /// <returns></returns>
    public bool TimerDone()
    {
        return (isPaused) ? pauseDifference == 0.0f : Time.time >= timeStamp + interval ? true : false;
    }

    /// <summary>
    /// Method for setting a timer
    /// </summary>
    /// <param name="_interval"></param>
    public void SetTimer(float _interval = 2)
    {
        timeStamp = Time.time;
        interval = _interval;
        isActive = true;
    }


    /// <summary>
    /// Method call for restarting the timer with the same interval
    /// </summary>
    public void RestartTimer()
    {
        SetTimer(interval);
    }

    /// <summary>
    /// Method call for stopping the timer.
    /// </summary>
    public void StopTimer()
    {
        isActive = false;
        timeStamp = interval;
    }

    /// <summary>
    /// Method call for pausing the timer
    /// </summary>
    /// <param name="pause"></param>
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