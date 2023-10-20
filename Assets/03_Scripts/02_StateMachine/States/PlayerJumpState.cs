using System;
using UnityEngine;

using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Bool  = System.Boolean;

[Serializable]
public sealed class PlayerJumpState : PlayerBaseState
{
    
    #region Variables

    [SerializeField] private F32 maxJumpHeight = 1.0f;
    [SerializeField] private F32 maxJumpTime   = 0.75f;

    private F32  _jumpUpSpeed;
    private Bool _hasJumped = false;

    #endregion

    #region Constructor

    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        
        SetupJumpVariables();
    }
    
    private void SetupJumpVariables()
    {
        F32 __timeToApex = maxJumpTime * 0.5f;
        Ctx.Gravity  = (-2             * maxJumpHeight) / pow(__timeToApex, 2);
        _jumpUpSpeed = (+2             * maxJumpHeight) / __timeToApex;
    }

    #endregion

    #region Enter/Exit

    public override void EnterState()
    {
        //Debug.Log("Entering Jump State");
        //Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        Ctx.Anims.SetTrigger(Ctx.JumpHash);
        
        _hasJumped = false;
    }
    public override void ExitState()
    {
        //Debug.Log("Exiting Jump State");
        //Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        
        _hasJumped = false;
    }
    
    #endregion

    #region Update
    
    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        //_jumpedThisFrame =  false;
        //Ctx.TimeSinceJumpRequested += deltaTime;
        
        // Calculate jump direction before ungrounding
        F32x3 __jumpDirection = Ctx.Motor.CharacterUp;

        if (Ctx.Motor.GroundingStatus is
            {
                FoundAnyGround:   true, 
                IsStableOnGround: false,
            })
        {
            __jumpDirection = Ctx.Motor.GroundingStatus.GroundNormal;
        }

        // Makes the character skip ground probing/snapping on its next update. 
        // NOTE: If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
        Ctx.Motor.ForceUnground();

        // Add to the return velocity and reset jump state
        currentVelocity += (Vector3)(__jumpDirection * _jumpUpSpeed - (F32x3)Vector3.Project(vector: currentVelocity, onNormal: Ctx.Motor.CharacterUp));
        //currentVelocity  += (Vector3)(Ctx.MoveInputVector * jumpScalableForwardSpeed);
        Ctx.JumpRequested          =  false;
        Ctx.JumpConsumed           =  true;
        Ctx.JumpedThisPhysicsFrame =  true;

        _hasJumped = true;
    }
    
    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }
    
    #endregion

    #region Switch States
    
    public override void CheckSwitchStates()
    {
        if (_hasJumped) //TODO: Could probably be done with JumpedThisPhysicsFrame? Not 100% sure.
        {
            SwitchState(Factory.Air());
        }
    }

    #endregion
    
}