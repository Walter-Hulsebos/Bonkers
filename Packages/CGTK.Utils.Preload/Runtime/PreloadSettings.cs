using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CGTK.Utils.Preload
{
    using CGTK.Utils.Extensions.Collections;
    using CGTK.Utils.Settings;
    
    [Settings(usage: SettingsUsage.RuntimeProject, displayPath: "CGTK/Preload")]
    public sealed class PreloadSettings : Settings<PreloadSettings>
    {
        [SerializeField] internal GameObject[] prefabs = Array.Empty<GameObject>();

        public static IEnumerable<GameObject> Prefabs => Instance.prefabs;
        
        protected override void OnValidate()
        {
            #if UNITY_EDITOR
            PlayerSettings.SetPreloadedAssets(assets: PlayerSettings.GetPreloadedAssets().AddRangeUnique(collection: prefabs).ClearAllNulls());
            #endif
        }
    }
}
