using System;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
      : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
        InitialSubState();
    }
    public override void EnterState()
    {
        HandleJump();
    }
    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        if(Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
        Ctx.IsJumping = false;
    }
    
    public override void UpdateState() 
    {
        CheckSwitchStates();
        HandleGravity();
    }
    public override void InitialSubState() 
    {
        if (!Ctx.IsMovementPressed) //&& !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else //if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        // else
        // {
        //     SetSubState(Factory.Run());
        // }
    }
    public override void CheckSwitchStates() 
    {
        if(Ctx.Motor.GroundingStatus.IsStableOnGround)
        {
            SwitchState(Factory.Grounded());
        }
    }
    void HandleJump()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);

        Ctx.IsJumping = true;
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
    }

    void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY += (Ctx.Gravity * fallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY =  Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);

        }
        else
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY += (Ctx.Gravity * Time.deltaTime);
            Ctx.AppliedMovementY =  (previousYVelocity + Ctx.CurrentMovementY) * .5f;

        }
    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, Single deltaTime)
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
}
