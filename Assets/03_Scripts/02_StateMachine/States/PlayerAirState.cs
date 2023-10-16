using UnityEngine;

public sealed class PlayerAirState : PlayerBaseState
{
    public PlayerAirState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
        InitialSubState(); 
    }

    public override void EnterState() 
    {
        //Ctx.CurrentMovementY = Ctx.Gravity;
        //Ctx.AppliedMovementY = Ctx.Gravity;
    }
    public override void ExitState() { }
    
    public override void UpdateState(ref Vector3 currentVelocity, float deltaTime) 
    {
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
    
    public override void InitialSubState() 
    {
        //jumping
        //rising
        //falling 
        
        if (Ctx.IsFalling)
        {
            SetSubState(Factory.Falling());
        }
        else
        {
            SetSubState(Factory.Rising());
        }
    }
    public override void CheckSwitchStates() 
    {
        if(Ctx.Motor.GroundingStatus.IsStableOnGround)
        {
            SwitchState(Factory.Grounded());
        }
    }
}