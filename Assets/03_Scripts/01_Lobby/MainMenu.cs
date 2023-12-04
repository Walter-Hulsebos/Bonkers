using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NobleConnect.Ice;

namespace Bonkers
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Image pressAnyButtonImage;
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject Logo;
        [SerializeField] private GameObject Text;
        [SerializeField] private SelectButtonFirst selectButtonFirstScript;


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
    }
}