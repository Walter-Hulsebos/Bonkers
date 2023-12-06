namespace Bonkers
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.Serialization;
    using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;
    #else
    using MonoBehaviour = UnityEngine.MonoBehaviour;
    #endif

    /// <summary>
    /// CRTP (Curiously Recurring Template Pattern) Multiton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Multiton<T> : MonoBehaviour where T : Multiton<T>
    {
        //TODO: Once Unity officially supports serialization of HashSet, use that instead of list.
        #if ODIN_INSPECTOR
        //[OdinSerialize]
        [ShowInInspector] [ReadOnly] [PropertyOrder(order: -1)]
        #endif
        public static List<T> Instances { get; [UsedImplicitly] private set; } = new();
        
        private void Initialize()
        {
            if (Instances.Contains(item: (T)this))
            {
                // //Check if unity is in editor mode, if so use destroyImmediate, else use destroy
                // //This is to prevent errors when exiting play mode in editor
                // #if UNITY_EDITOR
                // if(Application.isPlaying)
                // {
                //     Destroy(obj: gameObject);
                // }
                // else
                // {
                //     DestroyImmediate(obj: gameObject);
                // }
                // #else
                // Destroy(gameObject);
                // #endif
                //
                return; 
            }

            Instances.Add(item: (T)this);
        }

        private void DeInitialize()
        {
            Instances.Remove(item: (T)this);
        }

        #if UNITY_EDITOR
        protected virtual void Reset() 
        {
            Initialize();
        }
        #endif

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            DeInitialize();
        }
    }
}
