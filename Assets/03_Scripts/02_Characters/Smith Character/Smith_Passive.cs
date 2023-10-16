using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Characters
{
    public class Smith_Passive : MonoBehaviour
    {
        // Variables
        private bool coroutineStarted = false;
        public bool Get_Active
        {
            get { return coroutineStarted; }
        }

        [Range(100, 500)]
        [SerializeField] private float strengthDivider;
        [SerializeField] private float lengthMultiplier;
        [SerializeField] private float passiveStrength, passiveLength;
        public float Set_passiveFloat
        {
            set
            {
                passiveLength = 5f;
                passiveStrength += value / strengthDivider;

                if (!coroutineStarted)
                    StartCoroutine(passiveTimer());
            }
        }

        public float Get_Passive_Strenght
        {
            get { return passiveStrength; }
        }

        // End Variables

        /// <summary>
        /// Timer for the passive
        /// </summary>
        /// <returns></returns>
        private IEnumerator passiveTimer()
        {
            coroutineStarted = true;
            while (true)
            {
                if (passiveLength >= 0)
                {
                    passiveLength -= Time.deltaTime;
                    //passiveStrength -= Time.deltaTime;
                    yield return new WaitForSecondsRealtime(Time.deltaTime);
                }
                else
                {
                    passiveLength = 0;
                    passiveStrength = 0;
                    coroutineStarted = false;

                    yield break;
                }
            }
        }
    }
}
