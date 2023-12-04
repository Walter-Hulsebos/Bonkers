using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NobleConnect.Ice;
using UnityEngine.Audio;
using System.Security.Cryptography;
using ProjectDawn.Geometry2D;

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
        
        private bool inIntro = true;
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
    }
}