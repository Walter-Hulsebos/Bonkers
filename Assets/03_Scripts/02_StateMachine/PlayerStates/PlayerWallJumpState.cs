using System;
using UnityEngine;
using KinematicCharacterController;

using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

using F32 = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Bool = System.Boolean;

public sealed class PlayerWallJumpState : PlayerBaseState
{
    #region Variables

    private Bool _hasWallJumped = false;

    #endregion

    #region Constructor

    public PlayerWallJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    #endregion

    #region Enter/Exit

    public override void EnterState()
    {       
        Debug.Log("Entering Wall Jump State");
    }

    public override void ExitState()
    {
         Debug.Log("Exiting Wall Jump State");
    }

    #endregion

    #region Update

    protected override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
    {
       
    }

    protected override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime)
    {
        
    }

    #endregion

    #region Switch States

    public override void CheckSwitchStates()
    {
        // When the character touches the wall, he is automatically jumping from its surface.
        if (_hasWallJumped)
        {
            SwitchState(Factory.Air());
        }
        else if(!Ctx.WallJumpRequested && Ctx.Motor.GroundingStatus.FoundAnyGround)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.WallJumpRequested && !Ctx.Motor.GroundingStatus.IsStableOnGround){
            SwitchState(Factory.Jump());
        }
    }

    #endregion

   
}
