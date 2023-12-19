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
        [SerializeField] private InputActionReference accept;
        [SerializeField] private SelectButtonFirst selectButtonFirst;
        [SerializeField] private GameObject quitPopup;
        private bool popupActive;

        private void OnEnable()
        {
            backorClose.action.performed += CheckLeaveLobby;
            //accept.action.performed += ReturnToStatingScene;
            backorClose.action.Enable();
        }
        private void OnDisable()
        {
            backorClose.action.performed -= CheckLeaveLobby;
            //accept.action.performed -= ReturnToStatingScene;
            backorClose.action.Disable();
        }

        private void Update()
        {
            if(!popupActive && Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadSceneAsync("StartMenu");
            }
        }

        //private void ReturnToStatingScene(InputAction.CallbackContext context)
        //{
        //    if (!popupActive)
        //    {
        //        SceneManager.LoadSceneAsync("StartMenu");
        //    }
        //}

        private void CheckLeaveLobby(InputAction.CallbackContext context)
        {
            if (popupActive)
            {
                EventSystem.current.SetSelectedGameObject(null);
                popupActive = false;
                quitPopup.SetActive(true);
            }
            else
            {
                selectButtonFirst.SetSelectedFirst();
                popupActive = true;
                quitPopup.SetActive(false);
            }           
        }
    }
}
