namespace Bonkers.Characters.Shared
{

    using System;

    using CGTK.Utils.Extensions.Math.Math;
    using CGTK.Utils.UnityFunc;

    using KinematicCharacterController;

    using Sirenix.OdinInspector;

    using UnityEngine;

    using static UnityEngine.Mathf;

    using static Unity.Mathematics.math;

    using static ProjectDawn.Mathematics.math2;

//using static Bonkers.Characters.OrientationMethod;

    using F32   = System.Single;
    using F32x2 = Unity.Mathematics.float2;
    using F32x3 = Unity.Mathematics.float3;

    [Serializable]
    public sealed class WalkState : PlayerState
    {

        #region References

        #if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(groupName: "References", expanded: true)]
        #endif
        [SerializeField] private Animator animator;

        #if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(groupName: "References", expanded: true)]
        #endif
        [SerializeField] private KinematicCharacterMotor motor;

        #if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(groupName: "References", expanded: true)]
        #endif
        [SerializeField] private CharacterInputs inputs;
        
        #if UNITY_EDITOR
        private void Reset()
        {
            animator = GetComponent<Animator>();
            motor    = GetComponent<KinematicCharacterMotor>();
            inputs   = GetComponent<CharacterInputs>();
            
            if(animator == null) { animator = GetComponentInChildren<Animator>(); }
            if(motor    == null) { motor    = GetComponentInChildren<KinematicCharacterMotor>(); }
            if(inputs   == null) { inputs   = GetComponentInChildren<CharacterInputs>(); }
            
            if(animator == null) { Debug.LogWarning(message: $"No {nameof(Animator)} found on {name} or its children.",                context: this); }
            if(motor    == null) { Debug.LogWarning(message: $"No {nameof(KinematicCharacterMotor)} found on {name} or its children.", context: this); }
            if(inputs   == null) { Debug.LogWarning(message: $"No {nameof(CharacterInputs)} found on {name} or its children.",         context: this); }
        }
        #endif

        #endregion
        
        #region Variables

        [SerializeField] private F32 maxSpeed        = 10;
        [SerializeField] private F32 moveSharpness   = 15;
        [SerializeField] private F32 orientSharpness = 20;
        
        //[SerializeField] private AnimationStateReference walkAnim;

        #endregion
        

        #region Enter/Exit

        public override void OnEnter()
        {
            Debug.Log(message: "Entering Walk State");
            
            //animator.SetTrigger(name: walkAnim.StateName);

            //Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        }

        public override void OnExit()
        {
            Debug.Log(message: "Exiting Walk State");
            //Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        }

        #endregion

        #region Update

        public override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
        {
            F32 __currentVelocityMagnitude = currentVelocity.magnitude;

            F32x3 __effectiveGroundNormal = motor.GroundingStatus.GroundNormal;

            // Reorient velocity on slope
            currentVelocity = motor.GetDirectionTangentToSurface
                                  (direction: currentVelocity, surfaceNormal: __effectiveGroundNormal) * __currentVelocityMagnitude;

            F32x3 __moveInputVector = inputs.MoveInputVector;

            // Calculate target velocity
            F32x3 __inputRight = cross(x: __moveInputVector, y: motor.CharacterUp);

            F32x3 __reorientedInput = normalize(x: cross(x: __effectiveGroundNormal, y: __inputRight)) * length(x: __moveInputVector);

            F32x3 __targetMovementVelocity = __reorientedInput * maxSpeed;

            // Smooth movement Velocity
            currentVelocity = lerp(start: currentVelocity, end: __targetMovementVelocity, t: 1f - exp(x: -moveSharpness * deltaTime));
        }

        public override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime)
        {
            if (lengthsq(x: inputs.LookInputVector).Approx(compareTo: 0f)) return;

            // Smoothly interpolate from current to target look direction
            //Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Exp(power: -orientationSharpness * deltaTime));
            F32x3 __forward = motor.CharacterForward;
            F32x3 __look    = inputs.LookInputVector;

            F32x3 __smoothedLookInputDirection = normalizesafe(x: slerp(start: __forward, end: __look, t: 1 - exp(x: -orientSharpness * deltaTime)));

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            currentRotation = Quaternion.LookRotation(forward: __smoothedLookInputDirection, upwards: motor.CharacterUp);
        }

        #endregion

    }
}