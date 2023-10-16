using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
      : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
        InitialSubState(); 
    }

    public override void EnterState() { }
    public override void ExitState()  { }
    
    public override void UpdateState(ref Vector3 currentVelocity, float deltaTime) { }
    
    public override void InitialSubState() 
    {
        if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
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
