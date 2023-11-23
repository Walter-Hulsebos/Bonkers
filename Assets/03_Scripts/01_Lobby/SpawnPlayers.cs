using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
//using Bonkers.Controls;
using UnityEngine.SceneManagement;

namespace Bonkers.Lobby
{
    public class SpawnPlayers : MonoBehaviour
    {
        [SerializeField] private GameObject character;

        [SerializeField] private PlayerInput playerInput;

        [SerializeField] private GameObject playerInputManager;

        //private Dictionary<int, GameObject> connectedPlayers = new Dictionary<int, GameObject>();

        // called first
        private void OnEnable()
        {
            Debug.Log("OnEnable called");
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this);
        }

        // called second
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //foreach (var index in connectedPlayers)
            //{
            Instantiate(character);
            // CinemachineInputHandler[] x = character.GetComponentsInChildren<CinemachineInputHandler>();
            //
            // foreach (var item in x)
            // {
            //     item.playerInput = playerInput;
            // }


        }

        // called when the game is terminated
        private void OnDisable()
        {
            Debug.Log("OnDisable");
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

    }
}