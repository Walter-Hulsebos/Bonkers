using System;

using UnityEngine;

using F32 = System.Single;

public sealed class PlayerIdleState : PlayerBaseState
{
    #region Constructor

    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    
    #endregion
    
    #region Enter/Exit
    
    public override void EnterState()
    {
        //Debug.Log("Entering Idle State");
        //Ctx.Animator.SetBool(Ctx.IsIdleHash, value: true);
        Ctx.Anims.SetTrigger(Ctx.IdleHash);
    }

    public override void ExitState()
    {
        //Debug.Log("Exiting Idle State");
        //Ctx.Animator.SetBool(Ctx.IsIdleHash, value: false);
    }

    #endregion

    #region Update

    protected override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
    {
        currentVelocity = Vector3.zero;
    }

    protected override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime){}

    #endregion
    
}