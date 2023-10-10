using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class DialogueManager : CampaignSubManager
    {
        private Queue<string> lines;
        private UIHandler uiHandler;

        // Start is called before the first frame update
        public override void Start()
        {
            lines = new Queue<string>();
            uiHandler = CampaignManager.GetCampaignManager<UIHandler>();
        }


        public void ExecuteDialogue(Dialogue dialogue)
        {
            uiHandler.TogglePanel(CampaignManager.instance.GetDialoguePanel(), true);
            uiHandler.SetIcon(CampaignManager.instance.GetIcon(), dialogue.icon);
            CampaignManager.instance.GetDialogueName().text = dialogue.name;
            lines.Clear();

            foreach (string line in dialogue.lines)
            {
                lines.Enqueue(line);
            }

            DisplayLine();
        }

        public void DisplayLine()
        {
            Debug.Log("Gets in here");
            for (int i = 0; i < lines.Count; i++)
            {
                string[] newLines = lines.ToArray();
                Debug.Log(newLines[i]);
            }
            if (lines.Count == 0)
            {
                EndDialogue();
                return;
            }

            string line = lines.Dequeue();
            CampaignManager.instance.GetDialogueText().text = line;

        }

        public void EndDialogue()
        {
            uiHandler.TogglePanel(CampaignManager.instance.GetDialoguePanel(), false);
        }



        // Update is called once per frame
        public override void Update()
        {

        }
    }
}
