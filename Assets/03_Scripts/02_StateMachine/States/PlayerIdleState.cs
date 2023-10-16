using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    public override void EnterState() 
    {
        Ctx.Animator.SetBool(Ctx.IsIdleHash, value: true);
        
        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsIdleHash, value: false);
    }
    
    public override void UpdateState(ref Vector3 currentVelocity, float deltaTime) { }
    public override void InitialSubState() { }
    public override void CheckSwitchStates() 
    {
        // if (Ctx.IsMovementPressed)
        // {
        //     SwitchState(Factory.Walk());
        // }
    }
}
