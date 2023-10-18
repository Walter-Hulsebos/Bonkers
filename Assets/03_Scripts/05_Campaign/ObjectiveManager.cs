using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class ObjectiveManager : CampaignSubManager
    {
        public List<Objective> objective = new List<Objective>();
        public Objective currentObjective;
        private Timer disableUITimer = new Timer();


        // Start is called before the first frame update
        public override void Start()
        {
            SetNextObjective(0);
        }

        public void CompletedObjective()
        {
            SetNextObjective(currentObjective.order + 1);
        }

        public void SetNextObjective(int order)
        {
            currentObjective = GetObjectiveByOrder(order);

            if (!currentObjective)
            {
                CompletedAllObjectives();
                return;
            }

            //Set UI
            CampaignManager.instance.GetObjectiveText().text = currentObjective.description;


        }

        public Objective GetObjectiveByOrder(int order)
        {
            for (int i = 0; i < objective.Count; i++)
            {
                if (objective[i].order == order)
                {
                    return objective[i];
                }
            }
            return null;
        }

        public void CompletedAllObjectives()
        {
            //Set UI
            CampaignManager.instance.GetObjectiveText().text = "";
            //Disable UI

        }

        // Update is called once per frame
        public override void Update()
        {
            if (currentObjective != null)
            {
                if (currentObjective)
                {
                    currentObjective.CheckifHappening();
                }
            }
        }
    }
}
