using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Bonkers
{
    public class SimpleEnemyAI : Enemy
    {
        
        public int chaseDistance;
        private CampaignUtils utils;
       
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            utils = new CampaignUtils();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            if (utils.CalculateDistance(gameObject.transform.position, CampaignManager.instance.GetPlayer().transform.position) <= chaseDistance)
            {

            }
        }

        public void Movement()
        {


        }


        public void Chase()
        {


        }
    }
}
