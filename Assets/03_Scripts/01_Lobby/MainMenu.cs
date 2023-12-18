using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;
using System;

namespace Bonkers
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Image pressAnyButtonImage;
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject Logo;
        [SerializeField] private GameObject Text;
        [SerializeField] private SelectButtonFirst selectButtonFirstScript;
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private TMP_InputField MasterVolumeInputField;
        [SerializeField] private GameObject Optionstab;
        [SerializeField] private GameObject MainMenutab;
        [SerializeField] private GameObject LobbiesTab;
        [SerializeField] private InputActionReference backorClose;

        private bool inIntro = true;
        private bool inOptions = false;
        private bool inLobbies = false;

        private float holdBackButtonTimer = 0;
        private float requiredHoldTime = 0.5f;
        private bool backisHeld;

        private void OnEnable()
        {
            backorClose.action.started += CloseHoldStart;
            backorClose.action.canceled += CloseHoldEnd;
            backorClose.action.Enable();
        }
        private void OnDisable()
        {
            backorClose.action.started -= CloseHoldStart;
            backorClose.action.canceled -= CloseHoldEnd;
            backorClose.action.Disable();
        }
        public void Start() 
        { 
            selectButtonFirstScript = this.gameObject.GetComponent<SelectButtonFirst>();
        }

        
        public void Update()
        {
            if (Input.anyKey && inIntro)
            {
                inIntro = false;
                Text.SetActive(false);
                StartCoroutine(FadeofLogo());
            }
            MenusCloseTime();
        }

        public void NewCampaign()
        {
            SceneManager.LoadSceneAsync("T7_Campaign");
        }

        public void OnlinePlay() { SceneManager.LoadSceneAsync("Bootstrap"); }

        public void OnfflinePlay() { }

        public void Options() { }

        public void Quit() { Application.Quit(); }

        private void MenusCloseTime()
        {
                holdBackButtonTimer += Time.deltaTime;
                if (holdBackButtonTimer > requiredHoldTime && inOptions && backisHeld)
                {
                    SwitchOptionsOn();
                    selectButtonFirstScript.SetSelectedOptions();
                    SwithToMainMenu();
                backisHeld = false;
            }          

                if (holdBackButtonTimer > requiredHoldTime && inLobbies && backisHeld)
                {
                    SwitchLobbiesOn();
                    selectButtonFirstScript.SetSelectedLobbies();
                    LobbiesTab.SetActive(false);
                backisHeld = false;
            }            
        }

        private void CloseHoldStart(InputAction.CallbackContext context)
        {
            backisHeld = true;
            holdBackButtonTimer = 0;
        }

        private void CloseHoldEnd(InputAction.CallbackContext context)
        {
            backisHeld = false;
        }

        IEnumerator FadeofLogo()
        {
            Logo.GetComponent<Animator>().enabled = true;
            yield return new WaitForSecondsRealtime(0.55f);
            Logo.SetActive(false);
            yield return new WaitForSecondsRealtime(0.05f);
            pressAnyButtonImage.enabled = false;
            menu.SetActive(true);
            selectButtonFirstScript.SetSelectedFirst();
        }

        public void SwitchOptionsOn()
        {
            if(inOptions)
            {
                inOptions = false;
            }
            else
            {
                inOptions = true;
            }
        }

        public void SwitchLobbiesOn()
        {
            if(inLobbies)
            {
                inLobbies = false;
            }
            else
            { 
                inLobbies = true; 
            }
        }

        //  used to switch from the options menu (in the starting menu) to the main menu use of escape button
        private void SwithToMainMenu()
        {
            if(Optionstab && MainMenutab)
            {
                MainMenutab.SetActive(true);
                Optionstab.SetActive(false);
            }
        }

        public void IncreaseMasterVolume()
        {
            // Get the current volume
            audioMixer.GetFloat("volume", out float currentVolume);

            // Increase the volume (adjust the increment value as needed)
            float newVolume = Mathf.Clamp01(Mathf.Pow(10, (currentVolume + 5) / 10.0f));
            audioMixer.SetFloat("volume", Mathf.Log10(newVolume) * 10);
        }

        public void DecreaseMasterVolume()
        {
            // Get the current volume
            audioMixer.GetFloat("volume", out float currentVolume);

            // Decrease the volume (adjust the increment value as needed)
            float newVolume = Mathf.Clamp01(Mathf.Pow(10, (currentVolume - 5) / 10.0f));
            audioMixer.SetFloat("volume", Mathf.Log10(newVolume) * 10);
        }

        public void UpdateMasterVolumeFieldUI()
        {
            audioMixer.GetFloat("volume", out float currentVolume);
            MasterVolumeInputField.text = "" + currentVolume;
        }
    }
}