using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class BossAI : MonoBehaviour
    {
        [SerializeField] private float knockbackPercentage;
        [SerializeField] private int rotationNumber;
        [SerializeField] private int pattern;
        private Timer mechanicTimer = new Timer();
        private float range;
        private int phase;


        public int interpolationFramesCount = 45;
        int elapsedFrames = 0;

        private int[][] patterns = new int[4][];

        // Start is called before the first frame update
        public void Start()
        {
           
            patterns[0] = new int[3] { 0, 1, 2 };
            patterns[1] = new int[4] { 0, 1, 4, 2 };
            patterns[2] = new int[5] { 0, 1, 4, 4, 2 };
            patterns[3] = new int[5] { 0, 1, 3, 3, 2 };

            rotationNumber = 0;
            pattern = 0;

            mechanicTimer.SetTimer(5);
        }

        // Update is called once per frame
        public void Update()
        {
            //Normal Mechanics in here
            //Knockback check in here

            if (knockbackPercentage >= 50)
            {
                pattern = 3;
            }


            if (mechanicTimer.isActive && mechanicTimer.TimerDone())
            {
                mechanicTimer.StopTimer();
                ExecuteMechanic(GetPatternNumber(pattern, rotationNumber));
            }
        }




        public void ApplyPassive(float value)
        {
            knockbackPercentage -= value;
            if (knockbackPercentage < 0)
            {
                knockbackPercentage = 0;
            }
        }

        public void ExecuteMechanic(int mechanic)
        {
            switch (mechanic)
            {
                case (int)BossMechanics.Dash:
                    float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
                    Vector3 interpolatedPosition = Vector3.Lerp(gameObject.transform.position, CampaignManager.instance.player.transform.position, interpolationRatio);
                    elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);
                    Debug.DrawLine(Vector3.zero, Vector3.up, Color.green);
                    Debug.DrawLine(Vector3.zero, Vector3.forward, Color.blue);
                    Debug.DrawLine(Vector3.zero, interpolatedPosition, Color.yellow);
                    Debug.Log("Dash");
                    break;
                case (int)BossMechanics.Claw_Burst:
                    if (range <= 10)
                    {
                        CampaignManager.instance.GetPlayer().GetComponentInChildren<CampaignPlayer>().SetKnockBackPercentage(5, gameObject.transform.position);
                    }
                    Debug.Log("Claw burst");
                    break;
                case (int)BossMechanics.Bloody_Retreat:
                    Debug.Log("Bloody Retreat");
                    break;
                case (int)BossMechanics.Blood_Drain:
                    Debug.Log("Blood Drain");
                    break;
                case (int)BossMechanics.Blood_Slash:
                    Debug.Log("Blood Slash");
                    break;
                default:
                    break;
            }
            rotationNumber++;
            if (rotationNumber >= patterns[pattern].Length)
            {
                pattern++;
                if (pattern != 4)
                {
                    if (pattern >= patterns.Length)
                    {
                        pattern = 0;
                    }
                    pattern++;
                }
                rotationNumber = 0;
            }
            mechanicTimer.SetTimer(5);
        }


        public int GetPatternNumber(int pattern, int mechanic)
        {
            return patterns[pattern][mechanic];
        }

    }

    public enum BossMechanics
    {
        Dash,
        Claw_Burst,
        Bloody_Retreat,
        Blood_Drain,
        Blood_Slash
    }
}

