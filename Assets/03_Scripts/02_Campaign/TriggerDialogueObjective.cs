using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class TriggerDialogueObjective : Objective
    {
        private bool isCurrentObjective = false;


        private void OnTriggerEnter(Collider other)
        {
            if (isCurrentObjective && other.tag.Contains("Player"))
            {
                CampaignManager.GetCampaignManager<ObjectiveManager>().CompletedObjective();
                isCurrentObjective = false;
            }
        }


        public override void CheckifHappening()
        {
            isCurrentObjective = true;            
        }

    }
}
