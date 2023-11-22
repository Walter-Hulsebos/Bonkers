﻿namespace Bonkers.Characters.Shared
{
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
    using Bool  = System.Boolean;

    public sealed class GroundedState : PlayerState
    {

        #region Variables

        // private PlayerBaseState _subStateIdle;
        // private PlayerBaseState _subStateWalk;
        // private PlayerBaseState _subStateAttack;

        //private Bool _isAttacking1 = false;

        #endregion

        #region Constructor

        // public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        //     : base(currentContext, playerStateFactory) 
        // {
        //     IsRootState = true;
        //
        //     _subStateIdle = Factory.Idle();
        //     _subStateWalk = Factory.Walk();
        //     _subStateAttack = Factory.Special1();
        // }

        #endregion

        #region Enter/Exit

        public override void OnEnter()
        {
            Debug.Log("Entering Grounded State");
        }

        public override void OnExit()
        {
            Debug.Log("Exiting Grounded State");
        }

        #endregion

        #region Update

        public override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime) { }

        public override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime) { }

        #endregion

        #region Switch States

        // if (Ctx.Motor.GroundingStatus.IsStableOnGround)
        // {
        //     // if the player is grounded and attack is pressed, switch to attack state
        //     if (Ctx.Special1Requested)
        //     {
        //         SwitchSubState(_subStateAttack);
        //         Debug.Log("Still Attacking");
        //         
        //         _isAttacking1 = true;
        //     }
        //
        //     if (_isAttacking1 )
        //     {
        //         AnimatorStateInfo __animStateInfo = Ctx.Anims.GetCurrentAnimatorStateInfo(layerIndex: 0);
        //         F32 animPercentage = __animStateInfo.normalizedTime;
        //         
        //         if (animPercentage > 1.0f)
        //         {
        //             _isAttacking1 = false;
        //         }
        //     }
        //     
        //     if (!_isAttacking1 && !Ctx.Special1Requested)
        //     {
        //         if (Ctx.IsMovementPressed && !Ctx.Special1Requested)
        //         {
        //             SwitchSubState(_subStateWalk);
        //         }
        //         else if (!Ctx.IsMovementPressed && !Ctx.Special1Requested)
        //         {
        //             SwitchSubState(_subStateIdle);
        //         }
        //     }
        //     
        //
        //
        //     // if player is grounded and jump is pressed , switch to jump state
        //     if (Ctx.JumpRequested) //&& !Ctx.RequireNewJumpPress)
        //     {
        //         // Check if we're actually are allowed to jump
        //         if (!Ctx.JumpConsumed && Ctx.CanJumpAgain)
        //         {
        //             SwitchState(Factory.Jump());
        //         }
        //     }
        //    
        // }
        // else
        // {
        //     SwitchState(Factory.Air());
        // }

        #endregion

    }
}