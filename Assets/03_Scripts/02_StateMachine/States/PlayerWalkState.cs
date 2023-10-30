using System;

//using Bonkers.Characters;

using CGTK.Utils.Extensions.Math.Math;

using Sirenix.OdinInspector;

using UnityEngine;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

//using static Bonkers.Characters.OrientationMethod;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

[Serializable]
public sealed class PlayerWalkState : PlayerBaseState
{

    #region Variables
    
    [SerializeField] private F32 maxSpeed        = 10f;
    [SerializeField] private F32 moveSharpness   = 15f;
    [SerializeField] private F32 orientSharpness = 20f;

    #endregion

    #region Constructor
    
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) { }
    
    #endregion

    #region Enter/Exit
    
    public override void EnterState() 
    {
        //Debug.Log("Entering Walk State");
        Ctx.Anims.SetTrigger(Ctx.WalkHash);
        //Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
    }

    public override void ExitState()
    {
        //Debug.Log("Exiting Walk State");
        //Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
    }

    #endregion

    #region Update

    protected override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
    {
        F32 __currentVelocityMagnitude = currentVelocity.magnitude;

        F32x3 __effectiveGroundNormal = Ctx.Motor.GroundingStatus.GroundNormal;

        // Reorient velocity on slope
        currentVelocity = Ctx.Motor.GetDirectionTangentToSurface(direction: currentVelocity, surfaceNormal: __effectiveGroundNormal) * __currentVelocityMagnitude;

        F32x3 __moveInputVector = Ctx.MoveInputVector;
        
        // Calculate target velocity
        F32x3 __inputRight = cross(__moveInputVector, Ctx.Motor.CharacterUp);

        F32x3 __reorientedInput = normalize(cross(__effectiveGroundNormal, __inputRight)) * length(__moveInputVector);

        F32x3 __targetMovementVelocity = __reorientedInput * maxSpeed;

        // Smooth movement Velocity
        currentVelocity = lerp(currentVelocity, __targetMovementVelocity, t: 1f - exp(-moveSharpness * deltaTime));
    }

    protected override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime)
    {
        if(lengthsq(Ctx.LookInputVector).Approx(0f)) return;
        
        // Smoothly interpolate from current to target look direction
        //Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Exp(power: -orientationSharpness * deltaTime));
        F32x3 __forward = Ctx.Motor.CharacterForward;
        F32x3 __look    = Ctx.LookInputVector;
        
        F32x3 __smoothedLookInputDirection = normalizesafe(slerp(start: __forward, end: __look, t: 1 - exp(-orientSharpness * deltaTime)));

        // Set the current rotation (which will be used by the KinematicCharacterMotor)
        currentRotation = Quaternion.LookRotation(forward: __smoothedLookInputDirection, upwards: Ctx.Motor.CharacterUp);
    }
    
    #endregion
    
}