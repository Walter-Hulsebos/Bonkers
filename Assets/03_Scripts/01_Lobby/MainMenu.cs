using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using TMPro;

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


        private bool inIntro = true;
        private bool inOptions = false;

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

            // use of escape direct get only for testing
            if(Input.GetKeyDown(KeyCode.Escape) && inOptions)
            {
                SwitchOptionsOn();
                selectButtonFirstScript.SetSelectedOptions();
                SwithToMainMenu();
            }
        }

        public void NewCampaign()
        {
            SceneManager.LoadSceneAsync("T7_Campaign");
        }

        public void OnlinePlay() { SceneManager.LoadSceneAsync("Bootstrap"); }

        public void OnfflinePlay() { }

        public void Options() { }

        public void Quit() { Application.Quit(); }

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