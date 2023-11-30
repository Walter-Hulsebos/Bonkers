using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bonkers
{
    public class SceneExit : MonoBehaviour
    {
        public string sceneToLoad;

        private void OnTriggerEnter(Collider other)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
