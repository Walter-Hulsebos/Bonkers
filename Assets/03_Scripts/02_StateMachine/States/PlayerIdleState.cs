using System;

using UnityEngine;

using F32 = System.Single;

public sealed class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        Debug.Log("Entering Idle State");
        //Ctx.Animator.SetBool(Ctx.IsIdleHash, value: true);
        Ctx.Animator.SetTrigger(Ctx.IdleHash);
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle State");
        //Ctx.Animator.SetBool(Ctx.IsIdleHash, value: false);
    }

    protected override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
    {
        currentVelocity = Vector3.zero;
    }

    protected override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime)
    {
        
    }
}