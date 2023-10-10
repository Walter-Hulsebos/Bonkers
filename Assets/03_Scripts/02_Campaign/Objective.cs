using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class Objective : MonoBehaviour
    {
        public int order;
        [TextArea(3, 5)]
        public string description;
        // Start is called before the first frame update
        public virtual void Start()
        {
            CampaignManager.GetCampaignManager<ObjectiveManager>().objective.Add(this);
        }
        public virtual void CheckifHappening() 
        {
        
        }
    }
}
