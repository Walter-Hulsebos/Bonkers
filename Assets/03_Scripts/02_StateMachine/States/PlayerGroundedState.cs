using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
      : base(currentContext, playerStateFactory) {
        IsRootState = true;
        InitialSubState(); 
    }

    public override void EnterState() 
    {
        Ctx.CurrentMovementY = Ctx.GroundedGravity;
        Ctx.AppliedMovementY = Ctx.GroundedGravity;
    }
    public override void UpdateState() 
    {
        CheckSwitchStates();
    }
    public override void ExitState() { }
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
        // if player is grounded and jump is pressed , switch to jump state
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        {
            SwitchState(Factory.Jump());
        }
    }
}
