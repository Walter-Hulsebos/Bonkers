using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bonkers.Lobby
{
    public class LoadNextScene : MonoBehaviour
    {
        public void LoadScene()
        {
            SceneManager.LoadScene(1);
        }
    }
}
