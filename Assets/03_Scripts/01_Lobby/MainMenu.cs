using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Bonkers
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Image      pressAnyButtonImage;
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject LogoAndText;
        public                   void       Start() { }

        public void Update()
        {

            if (Keyboard.current.anyKey.isPressed)
            {
                LogoAndText.SetActive(false);
                pressAnyButtonImage.enabled = false;
                menu.SetActive(true);
            }
        }

        public void NewCampaign() { }

        public void OnlinePlay() { SceneManager.LoadSceneAsync("Bootstrap"); }

        public void OnfflinePlay() { }

        public void Options() { }

        public void Quit() { Application.Quit(); }
    }
}