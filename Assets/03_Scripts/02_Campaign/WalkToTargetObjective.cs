using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class WalkToTargetObjective : Objective
    {
        public override void CheckifHappening()
        {
            float distance = Vector3.Distance(CampaignManager.instance.GetPlayer().transform.position, gameObject.transform.position);
            if (distance < 1.5)
            {
                CampaignManager.GetCampaignManager<ObjectiveManager>().CompletedObjective();
            }
        }
    }
}
