namespace Bonkers.Shared
{
    using UnityEngine;
    
    public sealed class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }
}
