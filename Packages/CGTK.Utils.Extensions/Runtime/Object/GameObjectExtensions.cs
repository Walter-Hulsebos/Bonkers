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
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void DestroyAllGameObjects<T>(this IEnumerable<T> list) where T : Component
        {
            foreach (T __component in list)
            {
                DestroyGameObject(component: __component);
            }
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void DestroyAllComponents<T>(this IEnumerable<T> list) where T : Object
        {
            foreach (T __component in list)
            {
                DestroyComponent(component: __component);
            }
        }
        

        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void DestroyGameObject<T>(this T component) where T : Component
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(obj: component.gameObject);
            }
            else
            {
                DestroyImmediate(obj: component.gameObject);
            }
            #else
            Destroy(component.gameObject);
            #endif
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void DestroyComponent<T>(this T component) where T : Object
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(obj: component);
            }
            else
            {
                DestroyImmediate(obj: component);
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
                Object.DontDestroyOnLoad(target: obj);
            }
            #endif
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void InstantiatePrefabs(this IEnumerable<GameObject> prefabs)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(original: prefab);
            }
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void InstantiatePrefabs(this IEnumerable<GameObject> prefabs, Transform parent)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(original: prefab, parent: parent);
            }
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void InstantiatePrefabs(this IEnumerable<GameObject> prefabs, Vector3 position, Quaternion rotation)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(original: prefab, position: position, rotation: rotation);
            }
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void InstantiatePrefabs(this IEnumerable<GameObject> prefabs, Vector3 position, Quaternion rotation, Transform parent)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(original: prefab, position: position, rotation: rotation, parent: parent);
            }
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void InstantiatePrefabsDontDestroyOnLoad(this IEnumerable<GameObject> prefabs)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(original: prefab).DontDestroyOnLoad();
            }   
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void InstantiatePrefabsDontDestroyOnLoad(this IEnumerable<GameObject> prefabs, Transform parent)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(original: prefab, parent: parent).DontDestroyOnLoad();
            }   
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void InstantiatePrefabsDontDestroyOnLoad(this IEnumerable<GameObject> prefabs, Vector3 position, Quaternion rotation)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(original: prefab, position: position, rotation: rotation).DontDestroyOnLoad();
            }   
        }
        
        [PublicAPI]
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void InstantiatePrefabsDontDestroyOnLoad(this IEnumerable<GameObject> prefabs, Vector3 position, Quaternion rotation, Transform parent)
        {
            foreach (GameObject prefab in prefabs)
            {
                if(prefab == null) continue;
                
                Instantiate(original: prefab, position: position, rotation: rotation, parent: parent).DontDestroyOnLoad();
            }   
        }
    }
}
