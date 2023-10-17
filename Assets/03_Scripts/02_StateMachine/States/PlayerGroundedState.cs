using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
      : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;

        _subStateIdle = Factory.Idle();
        _subStateWalk = Factory.Walk();
    }
    
    private PlayerBaseState _subStateIdle;
    private PlayerBaseState _subStateWalk;

    public override void EnterState()
    {
        Debug.Log("Entering Grounded State");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Grounded State");
    }
    
    public override void UpdateState(ref Vector3 currentVelocity, float deltaTime) { }


    // public override void CheckSwitchSubStates()
    // {
    //     if (Ctx.Motor.GroundingStatus.IsStableOnGround)
    //     {
    //         if (Ctx.IsMovementPressed)
    //         {
    //             SetSubState(_subStateWalk);
    //         }
    //         else
    //         {
    //             SetSubState(_subStateIdle);
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("We're in air when we should be grounded");
    //     }
    // }

    public override void CheckSwitchStates() 
    {
        if (Ctx.Motor.GroundingStatus.IsStableOnGround)
        {
            // if player is grounded and jump is pressed , switch to jump state
            if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
            {
                SwitchState(Factory.Jump());
            }
            
            if (Ctx.IsMovementPressed)
            {
                SwitchSubState(_subStateWalk);
            }
            else
            {
                SwitchSubState(_subStateIdle);
            }
        }
        else
        {
            SwitchState(Factory.Air());
        }
    }
}
