namespace Extensions
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Unity.Burst;
    using Unity.Collections;

    using UnityEngine;

    using static System.Runtime.CompilerServices.MethodImplOptions;

    #if UNITY_EDITOR
    using Unity.Jobs;

    using UnityEditor;
    using UnityEditor.Animations;
    using BlendTree = UnityEditor.Animations.BlendTree;
    using AnimatorController = UnityEditor.Animations.AnimatorController;
    #endif
    
    using F32   = System.Single;
    using F32x2 = UnityEngine.Vector2;
    using F32x3 = UnityEngine.Vector3;
    
    using I32   = System.Int32;
    using I32x3 = UnityEngine.Vector3Int;
    
    using U32 = System.UInt16;

    public static class AnimationExtensions
    {
        #if UNITY_EDITOR
        public static F32 ConvertFrameToNormalizedTime(this AnimationClip clip, U32 frame)
        {
            F32 __frameAsSeconds = frame            / clip.frameRate;
            F32 __normalizedTime = __frameAsSeconds / clip.length;

            return __normalizedTime;
        }
        
        public static F32 ConvertNormalizedTimeToFrame(this AnimationClip clip, F32 normalizedTime)
        {
            F32 __frameAsSeconds = normalizedTime   * clip.length;
            F32 __frame          = __frameAsSeconds * clip.frameRate;

            return __frame;
        }
        
        public static F32 ConvertNormalizedTimeToSeconds(this AnimationClip clip, F32 normalizedTime)
        {
            F32 __frameAsSeconds = normalizedTime * clip.length;

            return __frameAsSeconds;
        }
        
        public static F32 ConvertSecondsToNormalizedTime(this AnimationClip clip, F32 seconds)
        {
            F32 __normalizedTime = seconds / clip.length;

            return __normalizedTime;
        }
        
        public static F32 ConvertFrameToSeconds(this AnimationClip clip, U32 frame)
        {
            F32 __frameAsSeconds = frame / clip.frameRate;

            return __frameAsSeconds;
        }
        
        public static U32 ConvertSecondsToFrame(this AnimationClip clip, F32 seconds)
        {
            F32 __frame = seconds * clip.frameRate;

            return (U32)__frame;
        }
        #endif
        
        #if UNITY_EDITOR
        /// <summary>
        /// Get the Currently used AnimatorController in the current AnimatorWindow and its related selected Animator, if any.
        /// </summary>
        /// <param name="controller">Current controller displayed in the Animator Window</param>
        /// <param name="animator">Current Animator component if inspecting one. May be null</param>
        public static void GetCurrentAnimatorAndController(out AnimatorController controller, out Animator animator)
        {
            Type         __animatorWindowType = Type.GetType(typeName: "UnityEditor.Graphs.AnimatorControllerTool, UnityEditor.Graphs");
            EditorWindow __window             = EditorWindow.GetWindow(windowType: __animatorWindowType);

            if (__animatorWindowType == null)
            {
                controller = null;
                animator   = null;
                return;
            }
            
            FieldInfo __animatorField = __animatorWindowType.GetField(name: "m_PreviewAnimator", bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);
            
            if (__animatorField is null)
            {
                throw new ArgumentNullException(paramName: "__animatorWindowType.GetField(\"m_PreviewAnimator\", BindingFlags.Instance | BindingFlags.NonPublic)");
            }
            
            animator = __animatorField.GetValue(obj: __window) as Animator;

            FieldInfo __controllerField = __animatorWindowType.GetField(name: "m_AnimatorController", bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

            if (__controllerField is null)
            {
                throw new ArgumentNullException(paramName: "__animatorWindowType.GetField(\"m_AnimatorController\", BindingFlags.Instance | BindingFlags.NonPublic)");
            }

            controller = __controllerField.GetValue(obj: __window) as AnimatorController;
        }
        #endif
        
        #if UNITY_EDITOR
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static AnimationClip FirstAvailableClip(this Motion motion)
        {
            AnimationClip __clip = motion as AnimationClip;
            
            if (__clip != null) return __clip;

            BlendTree __tree = motion as BlendTree;

            if (__tree == null) return null;

            foreach (ChildMotion __childMotion in __tree.children) 
            {
                // Should we be worried about `cycleOffset`? https://docs.unity3d.com/ScriptReference/Animations.AnimatorState-cycleOffset.html
                Motion    __child     = __childMotion.motion;
                BlendTree __childTree = __child as BlendTree;
                    
                if (__childTree != null) 
                {
                    AnimationClip __childClip = FirstAvailableClip(motion: __childTree);
                        
                    if (__childClip != null) return __childClip;
                }
                else 
                {
                    AnimationClip __childClip = __child as AnimationClip;
                        
                    if (__childClip != null) return __childClip;
                }
            }

            return null;
        }
        #endif

        #if UNITY_EDITOR
        public static void Scrub(GameObject gameObject, AnimationClip clip, F32 animTimePrimantissa)
        {
            EnsureAnimationMode();

            ScrubNoCheck(gameObject: gameObject, clip: clip, animTimePrimantissa: animTimePrimantissa);
        }
        #endif
        
        #if UNITY_EDITOR
        internal static void ScrubNoCheck(GameObject gameObject, AnimationClip clip, F32 animTimePrimantissa)
        {
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(gameObject: gameObject, clip: clip, time: animTimePrimantissa * clip.length);
            AnimationMode.EndSampling();
        }
        #endif
        
        #if UNITY_EDITOR
        public static void Scrub(StateMachineBehaviour stateMachineBehaviour, F32 animTimePrimantissa)
        {
            EnsureAnimationMode();
            
            GetCurrentAnimatorAndController(controller: out AnimatorController _, animator: out Animator __animator);

            StateMachineBehaviourContext[] __contexts = AnimatorController.FindStateMachineBehaviourContext(behaviour: stateMachineBehaviour);

            foreach (StateMachineBehaviourContext __context in __contexts) 
            {
                AnimatorState __state = __context.animatorObject as AnimatorState;
                if (__state == null) continue;

                AnimationClip __previewClip = __state.motion.FirstAvailableClip();
                if (__previewClip == null) continue;

                ScrubNoCheck(gameObject: __animator.gameObject, clip: __previewClip, animTimePrimantissa: animTimePrimantissa);
            }
        }
        #endif
        
        #if UNITY_EDITOR
        internal static void EnsureAnimationMode()
        {
            // Make sure we're in animation mode
            if (!AnimationMode.InAnimationMode())
            {
                AnimationMode.StartAnimationMode();
            }
        }
        #endif
        
        #if UNITY_EDITOR
        public static void OnionSkinning(GameObject gameObject, AnimationClip clip, F32 startFrame, F32 endFrame)
        {
            //TODO: Implement onion slicing.
            //Use jobs/burst to construct animation meshes?
        }
        
        [Serializable]
        public struct OnionSkinningData
        {
            public F32x3[] Vertices;
            public I32[]   Triangles;
            public F32x2[] UVs;
        }

        public static Vector3 InSideUnitCircleXZ()
        {
            Vector2 __circle = UnityEngine.Random.insideUnitCircle;
            return new Vector3(x: __circle.x, y: 0, z: __circle.y);
        }
        
        //Construct meshes for each frame of the animation clip using jobs/burst.
        public struct OnionSkinCreator : IJobParallelFor
        {
            #region Variables

            [NativeDisableParallelForRestriction] private NativeArray<F32x3> _verticies; // stores vertices;
            [NativeDisableParallelForRestriction] private NativeArray<I32>   _triangles;
            [NativeDisableParallelForRestriction] private NativeArray<F32x2> _uvs;

            #endregion
            
            #region Constructors
            
            public OnionSkinCreator(NativeArray<F32x3> verticies, NativeArray<I32> triangles, NativeArray<F32x2> uvs)
            {
                _verticies = verticies;
                _triangles = triangles;
                _uvs       = uvs;
            }
            
            //public OnionSkinCreator(Mesh
            
            #endregion
            
            public void Execute(int index)
            {
                
            }
        }
        
        public static void ConstructAnimationMeshes()
        {
            
        } 
        #endif
        
    }
}
