using System;

using Bonkers.Characters;

using CGTK.Utils.Extensions.Math.Math;

using Sirenix.OdinInspector;

using UnityEngine;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;

using static Bonkers.Characters.OrientationMethod;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

public sealed class PlayerAirState : PlayerBaseState
{

    #region Variables
    
    [SerializeField] private F32 maxSpeed      = 10f;
    [SerializeField] private F32 airAccelSpeed = 15f;
    [SerializeField] private F32 drag          = 0.1f;

    #endregion
    
    public PlayerAirState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
        
        _subStateFalling = Factory.Falling();
        _subStateRising  = Factory.Rising();
    }

    private PlayerBaseState _subStateFalling;
    private PlayerBaseState _subStateRising;

    public override void EnterState() 
    {
        Debug.Log("Entering Air State");
        //Ctx.CurrentMovementY = Ctx.Gravity;
        //Ctx.AppliedMovementY = Ctx.Gravity;
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Air State");
    }
    
    protected override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
    {
        F32x3 __moveInputVector = Ctx.MoveInputVector;

        if (lengthsq(__moveInputVector).Approx(0f)) return;

        F32x3 __addedVelocity = __moveInputVector * airAccelSpeed * deltaTime;

        F32x3 __currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(vector: currentVelocity, planeNormal: Ctx.Motor.CharacterUp);

        // Limit air velocity from inputs
        if (length(__currentVelocityOnInputsPlane) < maxSpeed)
        {
            // clamp addedVel to make total vel not exceed max vel on inputs plane
            F32x3 __newTotal = Vector3.ClampMagnitude(vector: __currentVelocityOnInputsPlane + __addedVelocity, maxLength: maxSpeed);
            __addedVelocity = __newTotal - __currentVelocityOnInputsPlane;
        }
        else
        {
            // Make sure added vel doesn't go in the direction of the already-exceeding velocity
            if (dot(__currentVelocityOnInputsPlane, __addedVelocity) > 0f)
            {
                __addedVelocity = Vector3.ProjectOnPlane(vector: __addedVelocity, planeNormal: normalize(__currentVelocityOnInputsPlane));
            }
        }

        // Prevent air-climbing sloped walls
        if (Ctx.Motor.GroundingStatus.FoundAnyGround)
        {
            if (dot((F32x3)currentVelocity + __addedVelocity, __addedVelocity) > 0f)
            {
                F32x3 __perpenticularObstructionNormal = normalize
                    (cross(cross(Ctx.Motor.CharacterUp, Ctx.Motor.GroundingStatus.GroundNormal), Ctx.Motor.CharacterUp));

                __addedVelocity = Vector3.ProjectOnPlane(vector: __addedVelocity, planeNormal: __perpenticularObstructionNormal);
            }
        }

        // Apply added velocity
        currentVelocity += (Vector3)__addedVelocity;

        // if (lengthsq(_moveInputVector) > 0f)
        // {
        //     F32x3 __addedVelocity = _moveInputVector * airAccelerationSpeed * deltaTime;
        //     
        //     F32x3 __currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(vector: currentVelocity, planeNormal: motor.CharacterUp);
        //
        //     // Limit air velocity from inputs
        //     if (length(__currentVelocityOnInputsPlane) < maxAirMoveSpeed)
        //     {
        //         // clamp addedVel to make total vel not exceed max vel on inputs plane
        //         F32x3 __newTotal = Vector3.ClampMagnitude(vector: __currentVelocityOnInputsPlane + __addedVelocity, maxLength: maxAirMoveSpeed);
        //         __addedVelocity = __newTotal - __currentVelocityOnInputsPlane;
        //     }
        //     else
        //     {
        //         // Make sure added vel doesn't go in the direction of the already-exceeding velocity
        //         if (dot(__currentVelocityOnInputsPlane, __addedVelocity) > 0f)
        //         {
        //             __addedVelocity = Vector3.ProjectOnPlane(vector: __addedVelocity, planeNormal: normalize(__currentVelocityOnInputsPlane));
        //         }
        //     }
        //
        //     // Prevent air-climbing sloped walls
        //     if (motor.GroundingStatus.FoundAnyGround)
        //     {
        //         if (dot((F32x3)currentVelocity + __addedVelocity, __addedVelocity) > 0f)
        //         {
        //             F32x3 __perpenticularObstructionNormal = normalize(cross
        //                                                         (
        //                                                                    cross
        //                                                                    (
        //                                                                        motor.CharacterUp,
        //                                                                        motor.GroundingStatus.GroundNormal
        //                                                                    ),
        //                                                                    motor.CharacterUp
        //                                                                ));
        //
        //             __addedVelocity = Vector3.ProjectOnPlane(vector: __addedVelocity, planeNormal: __perpenticularObstructionNormal);
        //         }
        //     }
        //
        //     // Apply added velocity
        //     currentVelocity += (Vector3)__addedVelocity;
        // }
        
        // bool __isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        //
        // const Single FALL_MULTIPLIER = 2.0f;
        //
        // float __previousYVelocity = Ctx.CurrentMovementY;
        //
        // if (__isFalling)
        // {
        //     Ctx.CurrentMovementY += (Ctx.Gravity * FALL_MULTIPLIER * Time.deltaTime);
        //     Ctx.AppliedMovementY =  Max((__previousYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);
        //
        // }
        // else
        // {
        //     Ctx.CurrentMovementY += (Ctx.Gravity * Time.deltaTime);
        //     Ctx.AppliedMovementY =  (__previousYVelocity + Ctx.CurrentMovementY) * .5f;
        // }
    }

    protected override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime)
    {
        
    }

    // public override void CheckSwitchSubStates()
    // {
    //     if(!Ctx.Motor.GroundingStatus.IsStableOnGround)
    //     {
    //         if (Ctx.IsFalling)
    //         {
    //             SetSubState(_subStateFalling);
    //         }
    //         else if (Ctx.IsRising)
    //         {
    //             SetSubState(_subStateRising);
    //         }
    //     }
    // }

    public override void CheckSwitchStates() 
    {
        if(Ctx.Motor.GroundingStatus.IsStableOnGround)
        {
            SwitchState(Factory.Grounded());
        }
        else
        {
            if (Ctx.IsFalling)
            {
                SwitchSubState(_subStateFalling);
            }
            else if (Ctx.IsRising)
            {
                SwitchSubState(_subStateRising);
            }
            
            if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
            {
                //Double Jump
            }
        }
        
        
        
        // else
        // {
        //     if (Ctx.IsFalling)
        //     {
        //         SwitchState(Factory.Falling());
        //     }
        //     else
        //     {
        //         SwitchState(Factory.Rising());
        //     }
        // }
    }
}