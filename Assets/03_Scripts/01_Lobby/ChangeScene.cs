using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bonkers
{
    public class ChangeScene : MonoBehaviour
    {
        public void GoToMainMenu() { SceneManager.LoadSceneAsync("MainMenu"); }
        public void GoToStartMenu() { SceneManager.LoadSceneAsync("StartMenu"); }
    }
}
