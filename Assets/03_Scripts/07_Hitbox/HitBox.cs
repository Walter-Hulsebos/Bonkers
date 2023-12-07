namespace Bonkers
{
    using System;

    using Extensions;

    using KinematicCharacterController;

    using UltEvents;

    using UnityEngine;
    
    using Unity.Mathematics;
    
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    using U16   = System.UInt16;
    using I32   = System.Int32;
    
    using F32   = System.Single;
    using F32x3 = Unity.Mathematics.float3;
    
    using Bool  = System.Boolean;
    
    public class HitBox3D : StateMachineBehaviour
    {
        private enum TimingMode
        {
            Frames,
            Seconds,
            #if ODIN_INSPECTOR
            [LabelText(text: "Primant")]
            #endif
            PrimantOfAnimation,
        }

        private enum Shape
        {
            Sphere,
            Box,
            Capsule,
        }
        
        private enum RegisterMode
        {
            [Tooltip(tooltip: "Register a hit only on the first moment of contact")]
            Initial,
            [Tooltip(tooltip: "Register a hit every frame of the animation (if there is contact)")]
            EveryFrame,
            [Tooltip(tooltip: "Register a hit every X frames of the animation (if there is contact)")]
            #if ODIN_INSPECTOR
            [LabelText(text: "Every X Frames")]
            #endif
            EveryXFrames,
            [Tooltip(tooltip: "Register a hit every X seconds of the animation (if there is contact)")]
            #if ODIN_INSPECTOR
            [LabelText(text: "Every X Seconds")]
            #endif
            EveryXSeconds,
        }

        #region Registry 
        
        #if ODIN_INSPECTOR
        //[EnumToggleButtons]
        [EnumPaging]
        [BoxGroup(@group: "Register", showLabel: false)] [LabelWidth(width: 120)]                             
        #endif                                                                                 
        [SerializeField] private RegisterMode register = RegisterMode.Initial;                 
                                                                                               
        #if ODIN_INSPECTOR                                                                                                                                          
        [ShowIf(condition: nameof(RegisterEveryXFrames))]                                      
        [BoxGroup(@group: "Register", showLabel: false)] [LabelText(text: "Frames")]  [LabelWidth(width: 120)]
        [SerializeField] private U16 registerEveryXFrames = 1;                                 
        #endif                                                                                 
                                                                                               
        #if ODIN_INSPECTOR                                                                     
        [ShowIf(condition: nameof(RegisterEveryXSeconds))]                                     
        [BoxGroup(@group: "Register", showLabel: false)] [LabelText(text: "Seconds")] [LabelWidth(width: 120)]
        [SerializeField] private F32 registerEveryXSeconds = 1.0f;                             
        #endif
        
        
        private Bool RegisterEveryXFrames   => register == RegisterMode.EveryXFrames;
        private Bool RegisterEveryXSeconds  => register == RegisterMode.EveryXSeconds;
        
        #endregion

        #region Check
        
        #if ODIN_INSPECTOR
        [FoldoutGroup(groupName: "Check")]
        [LabelWidth(width: 120)]
        #endif
        [SerializeField] private Shape shape = Shape.Sphere;
        
        //TODO: Show onion slices for attacks when in the editor of this statemachine behaviour.
        
        //TODO: Add begin and end of check position, and begin/end size and possibly direction of check. Curve for each. (OR ANIMATION!!!)
        
        #if ODIN_INSPECTOR
        [FoldoutGroup(groupName: "Check")]
        [LabelWidth(width: 120)]
        [MinMaxSlider(minValue: 0, maxValue: 1, showFields: true)]
        #endif
        [SerializeField] private Vector2 hitBoxCheckTime = new(x: 0.0f, y: 1.0f);
        
        private F32 CheckBeginPrimant => hitBoxCheckTime.x;
        private F32 CheckEndPrimant   => hitBoxCheckTime.y;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup(groupName: "Check")]
        [LabelWidth(width: 120)]
        #endif
        [SerializeField] private F32x3 startPosition = F32x3.zero;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup(groupName: "Check")]
        [LabelWidth(width: 120)]
        #endif
        [SerializeField] private F32x3 endPosition   = F32x3.zero;
        
        // #if ODIN_INSPECTOR
        // [FoldoutGroup("Check")]
        // [HorizontalGroup(@group: "Check/HitBoxCheckTime", width: 0.6f)] [LabelText(text: "Active")] [LabelWidth(120)]
        // [SerializeField] private TimingMode hitBoxCheckTimeMode = TimingMode.Frames;
        // #endif
        //
        // [SerializeField] private Vector2 hitBoxCheckTime = new(x: 0.0f, y: 1.0f);
        //
        // #if ODIN_INSPECTOR
        // [ShowIf(condition: nameof(HitBoxCheckTimeIsInFrames))]
        // [HorizontalGroup(@group: "Check/HitBoxDuration", width: 0.4f)] [HideLabel]
        // //[SuffixLabel(label: "frames", overlay: true)]
        // #endif
        // [SerializeField] private Vector2Int hitBoxCheckTimeFrames = new(x: 0, y: 1);
        //
        // #if ODIN_INSPECTOR
        // [ShowIf(condition: nameof(HitBoxCheckTimeIsInSeconds))]
        // [HorizontalGroup(@group: "Check/HitBoxDuration", width: 0.4f)] [HideLabel]
        // //[SuffixLabel(label: "seconds", overlay: true)] [MinValue(minValue: 0.01f)]
        // #endif
        // [SerializeField] private Vector2 hitBoxCheckTimeSeconds = new(x: 0.0f, y: 1.0f);
        //
        // #if ODIN_INSPECTOR
        // [ShowIf(condition: nameof(HitBoxCheckTimeIsPrimant))]
        // [HorizontalGroup(@group: "Check/HitBoxCheckTimeIsPrimant", width: 0.4f)] [HideLabel]
        // //[SuffixLabel(label: "primant", overlay: true)] [MinValue(minValue: 0.01f)] [MaxValue(maxValue: 1)]
        // #endif
        // [SerializeField] private Vector2 hitBoxCheckTimePrimant = new(x: 0.0f, y: 1.0f); 
        //
        // private Bool HitBoxCheckTimeIsInFrames  => hitBoxCheckTimeMode == TimingMode.Frames;
        // private Bool HitBoxCheckTimeIsInSeconds => hitBoxCheckTimeMode == TimingMode.Seconds;
        // private Bool HitBoxCheckTimeIsPrimant   => hitBoxCheckTimeMode == TimingMode.PrimantOfAnimation;
        
            // #region Duration
            //
            // #if ODIN_INSPECTOR
            // [FoldoutGroup("Check")]
            // [HorizontalGroup(@group: "Check/HitBoxDuration", width: 0.6f)] [LabelText(text: "Duration")] [LabelWidth(120)]
            // [SerializeField] private TimingMode hitBoxDurationMode = TimingMode.Frames;
            // #endif
            //
            // #if ODIN_INSPECTOR
            // [ShowIf(condition: nameof(HitBoxDurationModeIsFrames))]
            // [HorizontalGroup(@group: "Check/HitBoxDuration", width: 0.4f)] [HideLabel]
            // [SuffixLabel(label: "frames", overlay: true)]
            // #endif
            // [SerializeField] private U16 hitBoxDurationFrames = 1;
            //
            // #if ODIN_INSPECTOR
            // [ShowIf(condition: nameof(HitBoxDurationModeIsSeconds))]
            // [HorizontalGroup(@group: "Check/HitBoxDuration", width: 0.4f)] [HideLabel]
            // [SuffixLabel(label: "seconds", overlay: true)] [MinValue(minValue: 0.01f)]
            // #endif
            // [SerializeField] private F32 hitBoxDurationSeconds = 1.0f;
            //
            // #if ODIN_INSPECTOR
            // [ShowIf(condition: nameof(HitBoxDurationModeIsPrimant))]
            // [HorizontalGroup(@group: "Check/HitBoxDuration", width: 0.4f)] [HideLabel]
            // //[SuffixLabel(label: "primant", overlay: true)] [MinValue(minValue: 0.01f)] [MaxValue(maxValue: 1)]
            // #endif
            // [Range(min: 0, max: 1)]
            // [SerializeField] private F32 hitBoxDurationPrimant = 0.3f;
            //
            // private Bool HitBoxDurationModeIsFrames  => hitBoxDurationMode == TimingMode.Frames;
            // private Bool HitBoxDurationModeIsSeconds => hitBoxDurationMode == TimingMode.Seconds;
            // private Bool HitBoxDurationModeIsPrimant => hitBoxDurationMode == TimingMode.PrimantOfAnimation;    
            //
            // #endregion
        
        #endregion
        
        // #if ODIN_INSPECTOR
        // [FoldoutGroup("Knockback")]
        // [LabelWidth(120)]
        // #endif
        // [SerializeField] private F32x3 knockback       = Vector3.zero;
        //
        // // [Min(min: 0.0f)]
        // // [SerializeField] private F32   hitStunDuration = 0.0f;
        //
        // #region Hit Stun
        //
        // #if ODIN_INSPECTOR
        // [FoldoutGroup("Knockback")] [LabelText(text: "Stun Duration")] [LabelWidth(120)]
        // [SuffixLabel(label: "seconds", overlay: true)] [MinValue(minValue: 0.00f)]
        // #endif
        // [SerializeField] private F32 hitStunDurationSeconds = 0f;
        //
        // // #if ODIN_INSPECTOR
        // // [HorizontalGroup(@group: "Knockback/HitStunDuration", width: 0.6f)] [LabelText(text: "Stun Duration")] [LabelWidth(120)]
        // // [SerializeField] private TimingMode hitStunDurationMode = TimingMode.Seconds;
        // // #endif
        // //
        // // #if ODIN_INSPECTOR
        // // [ShowIf(condition: nameof(HitStunDurationModeIsFrames))]
        // // [HorizontalGroup(@group: "Knockback/HitStunDuration", width: 0.4f)] [HideLabel]
        // // [SuffixLabel(label: "frames", overlay: true)]
        // // #endif
        // // [SerializeField] private U16 hitStunDurationFrames = 0;
        // //
        // // #if ODIN_INSPECTOR
        // // [ShowIf(condition: nameof(HitStunDurationModeIsSeconds))]
        // // [HorizontalGroup(@group: "Knockback/HitStunDuration", width: 0.4f)] [HideLabel]
        // // [SuffixLabel(label: "seconds", overlay: true)] [MinValue(minValue: 0.00f)]
        // // #endif
        // // [SerializeField] private F32 hitStunDurationSeconds = 0f;
        // //
        // // #if ODIN_INSPECTOR
        // // [ShowIf(condition: nameof(HitStunDurationModeIsPrimant))]
        // // [HorizontalGroup(@group: "Knockback/HitStunDuration", width: 0.4f)] [HideLabel]
        // // //[SuffixLabel(label: "primant", overlay: true)] 
        // // #endif
        // // [Range(min: 0, max: 1)]
        // // [SerializeField] private F32 hitStunDurationPrimant = 0f;
        // //
        // // private Bool HitStunDurationModeIsFrames  => hitStunDurationMode == TimingMode.Frames;
        // // private Bool HitStunDurationModeIsSeconds => hitStunDurationMode == TimingMode.Seconds;
        // // private Bool HitStunDurationModeIsPrimant => hitStunDurationMode == TimingMode.PrimantOfAnimation;
        //
        // #endregion

        #region Callbacks
        
        #if ODIN_INSPECTOR
        [field:FoldoutGroup(groupName: "Callbacks")]
        #endif
        [field:Tooltip(tooltip: "Collider[] is the array of colliders that were hit. F32x3 is the position of the hit.")]
        [field:SerializeField] public UltEvent<Collider[], F32x3> OnHit { get; private set; } = null;
        
        #if ODIN_INSPECTOR
        [field:FoldoutGroup(groupName: "Callbacks")]
        #endif
        [field:Tooltip(tooltip: "Collider[] is the array of colliders that were hit. F32x3 is the position of the hit.")]
        [field:SerializeField] public UltEvent<Animator, AnimatorStateInfo> OnCheckStart { get; private set; } = null;
        
        #if ODIN_INSPECTOR
        [field:FoldoutGroup(groupName: "Callbacks")]
        #endif
        [field:Tooltip(tooltip: "Collider[] is the array of colliders that were hit. F32x3 is the position of the hit.")]
        [field:SerializeField] public UltEvent<Animator, AnimatorStateInfo> OnCheckEnd   { get; private set; } = null;

        #endregion

        private I32 _frame = 0;

        private KinematicCharacterMotor _motor;
        
        //TODO: Begin and end of check position, and begin/end size and possibly direction of check. Curve for each.

        #if ODIN_INSPECTOR
        private void Reset()
        {
            _hitBoxCheckTimeOld = hitBoxCheckTime;
        }

        private Vector2 _hitBoxCheckTimeOld;
        private void OnValidate()
        {
            //var animationEnd = 
            
            //Make sure the register every X frames is at least 1 frame long.
            registerEveryXFrames  = (U16)math.max(x: 1,     y: registerEveryXFrames);
            //Make sure the register every X seconds is at least 0.01 seconds long.
            registerEveryXSeconds = (F32)math.max(x: 0.01f, y: registerEveryXSeconds);
            
            //Make sure the hitbox duration is at least 1 frame long, or 0.01 seconds long for seconds and primant.
            // hitBoxDurationFrames  = (U16)math.max(x: 1,     y: hitBoxDurationFrames);
            // hitBoxDurationSeconds = (F32)math.max(x: 0.01f, y: hitBoxDurationSeconds);
            // hitBoxDurationPrimant = (F32)math.clamp(valueToClamp: hitBoxDurationPrimant, lowerBound: 0.01f, upperBound: 1f);
            
            
            //Make sure the hitstun duration is at least 0 frame long, or 0.01 seconds long for seconds and primant.
            // hitStunDurationFrames  = (U16)math.max(x: 0,  y: hitStunDurationFrames);
            // hitStunDurationSeconds = (F32)math.max(x: 0f, y: hitStunDurationSeconds);
            // hitStunDurationPrimant = (F32)math.clamp(valueToClamp: hitStunDurationPrimant, lowerBound: 0f, upperBound: 1f);
            
            //TODO: Make max clamped to the end of the animation.
            
            //TODO: Add conversion when switching between duration modes.

            //TODO: Only show the relevant animation state when dragging either beginning or end of hitbox duration.
            
            //Check whether X or Y of the hitbox duration has changed.
            //If so, show the scrub for the latest changed value.
            
            if(hitBoxCheckTime != _hitBoxCheckTimeOld)
            {
                if(!Mathf.Approximately(a: hitBoxCheckTime.x, b: _hitBoxCheckTimeOld.x))
                {
                    AnimationExtensions.Scrub(stateMachineBehaviour: this, animTimePrimantissa: hitBoxCheckTime.x);
                }
                else if(!Mathf.Approximately(a: hitBoxCheckTime.y, b: _hitBoxCheckTimeOld.y))
                {
                    AnimationExtensions.Scrub(stateMachineBehaviour: this, animTimePrimantissa: hitBoxCheckTime.y);
                }
            }
            
            // switch (hitBoxDurationMode)
            // {
            //     case TimingMode.Frames:
            //         break;
            //     case TimingMode.Seconds:
            //         break;
            //     case TimingMode.PrimantOfAnimation:
            //         AnimationExtensions.Scrub(stateMachineBehaviour: this, animTimePrimantissa: hitBoxDurationPrimant);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
            
            _hitBoxCheckTimeOld = hitBoxCheckTime;
        }
        
        //private void ConvertDurationMode
        #endif
        
        

        
        /// <summary> OnStateEnter is called when a transition starts and the state machine starts to evaluate this state </summary>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_motor == null)
            {
                _motor = animator.GetComponentInChildren<KinematicCharacterMotor>();    
            }
            
            _frame = 0;

            
        }
        
        private Bool _isChecking = false;

        /// <summary> OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks </summary>
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            F32 __normalizedTime     = stateInfo.normalizedTime;
            F32 __currentAnimPrimant = __normalizedTime % 1.0f;
            
            if ((__normalizedTime >= CheckEndPrimant)   && _isChecking == false)
            {
                _isChecking = true;
                OnCheckStart.Invoke(parameter0: animator, parameter1: stateInfo);
            }
            if ((__normalizedTime >= CheckBeginPrimant) && _isChecking == true)
            {
                _isChecking = false;
                OnCheckEnd.Invoke(parameter0: animator, parameter1: stateInfo);
            }

            if (_isChecking)
            {
                F32 __checkPrimant = math.remap(srcStart: CheckBeginPrimant, srcEnd: CheckEndPrimant, dstStart: 0.0f, dstEnd: 1.0f, x: __currentAnimPrimant);
                
                
            }
            
        }
        
        /// <summary> OnStateExit is called when a transition ends and the state machine finishes evaluating this state </summary>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Make sure that the hit ends when the animation ends, even if the hitbox duration is longer.
            //Skip if the check end time is less than the animation end time.
            
            
            // if (stateInfo.normalizedTime >= 1.0f)
            // {
            //     OnCheckEnd.Invoke(animator, stateInfo);
            // }
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
