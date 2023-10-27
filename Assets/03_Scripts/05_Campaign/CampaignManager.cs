using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Bonkers
{
    public class CampaignManager : MonoBehaviour
    {

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI dialogueName;
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private Image characterIcon;


        public VideoClip intro;

        public GameObject player;

        [Header("Timed Events")]
        [SerializeField] private TextMeshProUGUI timedEvent;



        public static CampaignManager instance { get; private set; }
        private static CampaignSubManager[] subManagers;


        CampaignManager()
        {
            instance = this;

            subManagers = new CampaignSubManager[]
            {
                new UIHandler(),
                new DialogueManager(),
                new TimedEventManager(),
                new ObjectiveManager(),

        };
        }

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

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
        /// <summary>
        /// Method to load level by name, if teleporting isnt required, use Vector3.zero
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="locationToTeleportTo"></param>
        /// <param name="shouldTeleport"></param>
        public void LoadLevelByName(Levels sceneName, Vector3 locationToTeleportTo, bool shouldTeleport = false)
        {
            SceneManager.LoadSceneAsync(sceneName.ToString());

            if (shouldTeleport)
            {
                player.transform.position = locationToTeleportTo;
            }
        }




        public void ToNextScene()
        {

            {
                LoadLevelByName(Levels.Campaign_02, Vector3.zero);
            }
        }



        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
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
        #region ForwardToManager
        public void ForwardNextSentence()
        {
            GetCampaignManager<DialogueManager>().DisplayLine();
        }

        public void ForwardTimedEvent()
        {
            GetCampaignManager<TimedEventManager>().ReactToTimedEvent();
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
        public TextMeshProUGUI GetObjectiveText()
        {
            return objectiveText;
        }

        public GameObject GetDialoguePanel()
        {
            return dialoguePanel;
        }
        public Image GetIcon()
        {
            return characterIcon;
        }
        public TextMeshProUGUI GetTimedEventText()
        {
            return timedEvent;
        }
        public GameObject GetPlayer()
        {
            return player;
        }
    }
}
public enum Levels
{
    Campaign_02,
}
