using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

using UnityEngine;
using static UnityEngine.Object;

using JetBrains.Annotations;

namespace CGTK.Utils.Extensions
{
    public static partial class GameObjectExtensions
    {
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllGameObjects<T>(this IEnumerable<T> list) where T : Component
        {
            foreach (T __component in list)
            {
                DestroyGameObject(__component);
            }
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllComponents<T>(this IEnumerable<T> list) where T : Object
        {
            foreach (T __component in list)
            {
                DestroyComponent(__component);
            }
        }
        

        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void DestroyGameObject<T>(this T component) where T : Component
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(component.gameObject);
            }
            else
            {
                DestroyImmediate(component.gameObject);
            }
            #else
            Destroy(component.gameObject);
            #endif
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void DestroyComponent<T>(this T component) where T : Object
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(component);
            }
            else
            {
                DestroyImmediate(component);
            }
            #else
            Destroy(component);
            #endif
        }
        
        public static void DontDestroyOnLoad(this Object obj)
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.DontDestroyOnLoad(obj);
            }
            #endif
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void InstantiatePrefabs(this IEnumerable<GameObject> prefabs)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(prefab);
            }
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void InstantiatePrefabs(this IEnumerable<GameObject> prefabs, Transform parent)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(prefab, parent);
            }
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void InstantiatePrefabs(this IEnumerable<GameObject> prefabs, Vector3 position, Quaternion rotation)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(prefab, position, rotation);
            }
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void InstantiatePrefabs(this IEnumerable<GameObject> prefabs, Vector3 position, Quaternion rotation, Transform parent)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(prefab, position, rotation, parent);
            }
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void InstantiatePrefabsDontDestroyOnLoad(this IEnumerable<GameObject> prefabs)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(prefab).DontDestroyOnLoad();
            }   
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void InstantiatePrefabsDontDestroyOnLoad(this IEnumerable<GameObject> prefabs, Transform parent)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(prefab, parent).DontDestroyOnLoad();
            }   
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void InstantiatePrefabsDontDestroyOnLoad(this IEnumerable<GameObject> prefabs, Vector3 position, Quaternion rotation)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(prefab, position, rotation).DontDestroyOnLoad();
            }   
        }
        
        [PublicAPI]
        [MethodImpl(AggressiveInlining)]
        public static void InstantiatePrefabsDontDestroyOnLoad(this IEnumerable<GameObject> prefabs, Vector3 position, Quaternion rotation, Transform parent)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(prefab, position, rotation, parent).DontDestroyOnLoad();
            }   
        }
    }
}
