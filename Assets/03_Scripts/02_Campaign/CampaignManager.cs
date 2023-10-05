using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Bonkers
{
    public class CampaignManager : MonoBehaviour
    {

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI dialogueName;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Button continueButton;
        [SerializeField] private Image characterIcon;

        public static CampaignManager instance { get; private set; }
        private static CampaignSubManager[] subManagers;

        CampaignManager()
        {
            instance = this;
            subManagers = new CampaignSubManager[]
            {
                new UIHandler(),
                new DialogueManager(),
            };
        }

        public static T GetCampaignManager<T>() where T : CampaignSubManager
        {
            for (int i = 0; i < subManagers.Length; i++)
            {
                if (typeof(T) == subManagers[i].GetType())
                {
                    return (T)subManagers[i];
                }
            }
            return default(T);
        }


        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < subManagers.Length; i++)
            {
                subManagers[i].Start();
            }
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < subManagers.Length; i++)
            {
                subManagers[i].Update();
            }
        }
        #region Dialogue
        public void ForwardNextSentence(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                GetCampaignManager<DialogueManager>().DisplayLine();
            }
        }

        #endregion


        public TextMeshProUGUI GetDialogueText()
        {
            return dialogueText;
        }

        public TextMeshProUGUI GetDialogueName()
        {
            return dialogueName;
        }

        public GameObject GetDialoguePanel()
        {
            return dialoguePanel;
        }

        public EventSystem GetEventSystem()
        {
            return eventSystem;
        }
        public Button GetContinueButton()
        {
            return continueButton;
        }
        public Image GetIcon()
        {
            return characterIcon;
        }

    }
}
