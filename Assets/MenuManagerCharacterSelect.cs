using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Bonkers
{
    public class MenuManagerCharacterSelect : MonoBehaviour
    {
        [SerializeField] private InputActionReference backorClose;
        [SerializeField] private GameObject quitPopup;
        private bool popupActive;

        private void OnEnable()
        {
            backorClose.action.performed += CheckLeaveLobby;
            backorClose.action.Enable();
        }
        private void OnDisable()
        {
            backorClose.action.performed -= CheckLeaveLobby;
            backorClose.action.Disable();
        }

        private void Update()
        {
            if(!popupActive && Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadSceneAsync("StartMenu");
            }
        }

        private void CheckLeaveLobby(InputAction.CallbackContext context)
        {
            if (popupActive)
            {
                popupActive = false;
                quitPopup.SetActive(true);
            }
            else
            {
                popupActive = true;
                quitPopup.SetActive(false);
            }           
        }
    }
}
