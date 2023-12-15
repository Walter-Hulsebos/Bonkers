using System;

//For Double Jump Controlls
using UnityEngine.InputSystem;

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
using Bonkers.Controls;
using KinematicCharacterController;

public sealed class PlayerAirState : PlayerBaseState
{

    #region Variables
    
    [SerializeField] private F32 maxSpeedForInputs = 10f;
    [SerializeField] private F32 accSpeed          = 15f;
    [SerializeField] private F32 drag              = 0.1f;
    [SerializeField] private F32 orientSharpness   = 20f;
    


    
    private PlayerBaseState _subStateFalling;
    private PlayerBaseState _subStateRising;
    private PlayerBaseState _subStateWallJump;

    //Double Jump Requirements
    public Controls playerControls;
    public InputAction doubleJumpAction;

    private bool DoubleJumpAvailable;

    //Wall Sliding Requirements
    private bool EnableWallSliding;

    #endregion

    #region Constructor

    public PlayerAirState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
        
        _subStateFalling = Factory.Falling();
        _subStateRising  = Factory.Rising();
        _subStateWallJump = Factory.WallJump();
    }

    #endregion

    #region Enter/Exit
    
    public override void EnterState() 
    {
        //set input actions
        playerControls = new Controls();

        doubleJumpAction = playerControls.Gameplay.Jump;
        doubleJumpAction.Enable();

        //Debug.Log("Entering Air State");
        Debug.Log("Has entered into Air state");
    }

    public override void ExitState()
    {
        //Debug.Log("Exiting Air State");
        Debug.Log("Exiting Air State");
        doubleJumpAction.Disable();
    }

    #endregion

    #region Update

    protected override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
    {
        F32x3 __moveInputVector = Ctx.MoveInputVector;

        if (lengthsq(__moveInputVector).Approx(0f)) return;

        F32x3 __addedVelocity = __moveInputVector * accSpeed * deltaTime;

        F32x3 __currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(vector: currentVelocity, planeNormal: Ctx.Motor.CharacterUp);

         // Limit air velocity from inputs
         if (length(__currentVelocityOnInputsPlane) < maxSpeedForInputs)
         {
             // clamp addedVel to make total vel not exceed max vel on inputs plane
             F32x3 __newTotal = Vector3.ClampMagnitude(vector: __currentVelocityOnInputsPlane + __addedVelocity, maxLength: maxSpeedForInputs);
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
        
        // Apply drag
        __addedVelocity *= 1f / (1f + (drag * deltaTime));

        // Apply added velocity
        currentVelocity += (Vector3)__addedVelocity;
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
    protected override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        base.OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);

        if (hitCollider.CompareTag("WallTest"))
        {
             // Transition to the wall slide state
            EnableWallSliding = true;
            Debug.Log("Player on wall");            
        }
    }

    #endregion

    #region Switch States

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

            //Initiating double jump
            doubleJumpAction.started += Button =>
            {
                Debug.Log("Jumped");
                Debug.Log(Ctx.DoubleJumpAvailable + " doubleJump");
                if (Ctx.DoubleJumpAvailable == true)
                {
                    Debug.Log("JumpedAgain");

                    Ctx.DoubleJumpAvailable = false;
                    SwitchState(Factory.ExtraJump());
                }
            };

            //Initiating wall sliding
            if(EnableWallSliding == true)
            {
                SwitchState(Factory.WallSliding());
            }
        }
    }

    #endregion
}