using UnityEngine;

namespace CGTK.Utils.Preload
{
    using CGTK.Utils.Extensions;

    internal static class Preloader
    {
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            PreloadSettings.Prefabs.InstantiatePrefabsDontDestroyOnLoad();
        }
    }
}