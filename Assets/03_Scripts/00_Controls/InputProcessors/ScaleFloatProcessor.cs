namespace Bonkers.Controls
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    
    using F32 = System.Single;

    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    #endif
    public class ScaleFloatProcessor : InputProcessor<F32>
    {
        [Tooltip("Value to scale the input by.")]
        public F32 scaleBy = 0;

        public override F32 Process(F32 value, InputControl control) => value * scaleBy;

        #region Static Constructor

        #if UNITY_EDITOR
        static ScaleFloatProcessor()
        {
            Initialize();
        }
        #endif
        
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            InputSystem.RegisterProcessor<ScaleFloatProcessor>();
        }

        #endregion
    }
}