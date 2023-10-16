using System;
using UnityEngine;

public sealed class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
      : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
        InitialSubState();
    }
    
    private Boolean _isJumping = false;
    
    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        _isJumping = true;
        
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
    }
    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        _isJumping = false;
        
        if(Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
    }
    
    public override void UpdateState(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = new Vector3(x: currentVelocity.x, Ctx.CurrentMovementY, z: currentVelocity.z);
        
        // // Add move input
        // if (currentMovementInput.sqrMagnitude > 0f)
        // {
        //     __targetMovementVelocity = (currentMovementInput * moveAirMaxSpeed);
        //
        //     // Prevent climbing on un-stable slopes with air movement
        //     if (Motor.GroundingStatus.FoundAnyGround)
        //     {
        //         Vector3 perpenticularObstructionNormal = Vector3.Cross(lhs: Vector3.Cross(lhs: Motor.CharacterUp, rhs: Motor.GroundingStatus.GroundNormal), rhs: Motor.CharacterUp).normalized;
        //         __targetMovementVelocity = Vector3.ProjectOnPlane(vector: __targetMovementVelocity, planeNormal: perpenticularObstructionNormal);
        //     }
        //
        //     Vector3 velocityDiff = Vector3.ProjectOnPlane(vector: __targetMovementVelocity - currentVelocity, planeNormal: Vector3.up * Gravity);
        //     currentVelocity += velocityDiff * (moveAirAccSpeed * deltaTime);
        // }
        //
        // // Gravity
        // //currentVelocity += Gravity * deltaTime;
        //
        // // Drag
        // //currentVelocity *= (1f / (1f + (Drag * deltaTime)));
    }
    
    public override void InitialSubState()   { }

    public override void CheckSwitchStates()
    {
        if (_isJumping)
        {
            SwitchState(Factory.Air());
        }
    }

    // void HandleGravity()
    // {
    //     bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
    //     float fallMultiplier = 2.0f;
    //
    //     if (isFalling)
    //     {
    //         float previousYVelocity = Ctx.CurrentMovementY;
    //         Ctx.CurrentMovementY += (Ctx.Gravity * fallMultiplier * Time.deltaTime);
    //         Ctx.AppliedMovementY =  Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);
    //
    //     }
    //     else
    //     {
    //         float previousYVelocity = Ctx.CurrentMovementY;
    //         Ctx.CurrentMovementY += (Ctx.Gravity * Time.deltaTime);
    //         Ctx.AppliedMovementY =  (previousYVelocity + Ctx.CurrentMovementY) * .5f;
    //
    //     }
    // }

}
