using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class DialogueTriggerObject : MonoBehaviour
    {
        public Dialogue dialogue;

       /* public void TriggerDialogue()
        {
            CampaignManager.GetCampaignManager<DialogueManager>().ExecuteDialogue(dialogue);
        }
       */

        public void OnTriggerEnter(Collider other)
        {
            if (other.tag.Contains("Player"))
            {
                CampaignManager.GetCampaignManager<DialogueManager>().ExecuteDialogue(dialogue);
            }
        }

    }
}
