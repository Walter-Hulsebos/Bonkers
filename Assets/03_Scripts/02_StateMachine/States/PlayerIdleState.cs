using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        Debug.Log("Entering Idle State");
        Ctx.Animator.SetBool(Ctx.IsIdleHash, value: true);
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle State");
        Ctx.Animator.SetBool(Ctx.IsIdleHash, value: false);
    }

    public override void UpdateState(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = Vector3.zero;
    }
    public override void CheckSwitchStates() 
    {
        if (Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
    }
}
