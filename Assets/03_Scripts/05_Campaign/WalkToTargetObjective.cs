using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class WalkToTargetObjective : Objective
    {

        private bool isHappening;
        public override void CheckifHappening()
        {
            isHappening = true;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (isHappening)
            {
                if (other.transform.CompareTag("Player"))
                {
                    CampaignManager.GetCampaignManager<ObjectiveManager>().CompletedObjective();
                }
               
            }
        }
    }
}
