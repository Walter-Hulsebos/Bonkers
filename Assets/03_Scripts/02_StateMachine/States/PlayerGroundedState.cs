using System;

//using Bonkers.Characters;

using CGTK.Utils.Extensions.Math.Math;

using Sirenix.OdinInspector;

using UnityEngine;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

//using static Bonkers.Characters.OrientationMethod;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

public sealed class PlayerGroundedState : PlayerBaseState
{
    
    #region Variables
    
    private PlayerBaseState _subStateIdle;
    private PlayerBaseState _subStateWalk;
    
    #endregion

    #region Constructor

    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;

        _subStateIdle = Factory.Idle();
        _subStateWalk = Factory.Walk();
    }

    #endregion
    
    #region Enter/Exit
    
    public override void EnterState()
    {
        //Debug.Log("Entering Grounded State");
    }

    public override void ExitState()
    {
        //Debug.Log("Exiting Grounded State");
    }
    
    #endregion

    #region Update

    protected override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime) { }
    
    protected override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime) { }

    #endregion
    
    #region Switch States
    
    public override void CheckSwitchStates() 
    {
        if (Ctx.Motor.GroundingStatus.IsStableOnGround)
        {
            // if player is grounded and jump is pressed , switch to jump state
            if (Ctx.JumpRequested) //&& !Ctx.RequireNewJumpPress)
            {
                // Check if we're actually are allowed to jump
                if (!Ctx.JumpConsumed && Ctx.CanJumpAgain)
                {
                    SwitchState(Factory.Jump());
                }
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
    
    #endregion
    
}
