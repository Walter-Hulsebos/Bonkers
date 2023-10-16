using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Bonkers
{
    public class MainMenuDisplay : MonoBehaviour
    {

        [SerializeField] private string gameplaySceneName = "Gameplay";

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);

        }

        public void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
