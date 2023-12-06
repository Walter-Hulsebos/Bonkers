using System;

//using Bonkers.Characters;

using CGTK.Utils.Extensions.Math.Math;

using KinematicCharacterController;

using Sirenix.OdinInspector;

using UnityEngine;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;
using static PlayerStateMachine.Character;

//using static Bonkers.Characters.OrientationMethod;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Bool  = System.Boolean;

public sealed class PlayerGroundedState : PlayerBaseState
{
    
    #region Variables
    
    private PlayerBaseState _subStateIdle;
    private PlayerBaseState _subStateWalk;

    #region Druid
    private PlayerBaseState _subStateBasicAttack_Druid;
    private PlayerBaseState _subStateAttackSpecial1_Druid;
    private PlayerBaseState _subStateAttackSpecial2_Druid;
    private PlayerBaseState _subStateAttackSpecial3_Druid;
    #endregion

    #region Smith
    private PlayerBaseState _subStateBasicAttack_Smith;
    private PlayerBaseState _subStateAttackSpecial1_Smith;
    private PlayerBaseState _subStateAttackSpecial2_Smith;
    private PlayerBaseState _subStateAttackSpecial3_Smith;
    #endregion

    #region CatWoman
    private PlayerBaseState _subStateBasicAttack_CatWoman;
    private PlayerBaseState _subStateAttackSpecial1_CatWoman;
    private PlayerBaseState _subStateAttackSpecial2_CatWoman;
    private PlayerBaseState _subStateAttackSpecial3_CatWoman;
    #endregion

    private Bool _isAttacking = false;

    private Bool _isKnockedBack = false;
    
    #endregion

    #region Constructor

    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;

        _subStateIdle = Factory.Idle();
        _subStateWalk = Factory.Walk();

        #region Druid Attack
        _subStateBasicAttack_Druid    = Factory.DruidBasicAttack();
        _subStateAttackSpecial1_Druid = Factory.DruidSpecial1();
        _subStateAttackSpecial2_Druid = Factory.DruidSpecial2();
        #endregion

        #region Smith Attack
        _subStateBasicAttack_Smith =    Factory.SmithBasicAttack();
        _subStateAttackSpecial1_Smith = Factory.SmithSpecial1();
        _subStateAttackSpecial2_Smith = Factory.SmithSpecial2();
        #endregion

        #region CatWoman Attack
        _subStateBasicAttack_CatWoman = Factory.CatWomanBasicAttack();
        _subStateAttackSpecial1_CatWoman = Factory.CatWomanSpecial1();
        _subStateAttackSpecial2_CatWoman = Factory.CatWomanSpecial2();
        #endregion
    }

    #endregion

    #region Enter/Exit

    public override void EnterState()
    {
        Debug.Log($"Entering Grounded State - frame ({Time.frameCount})", context: Ctx);
        
        _isKnockedBack = false;
    }

    public override void ExitState()
    {
        Debug.Log($"Exiting Grounded State - frame ({Time.frameCount})", context: Ctx);
    }

    #endregion

    #region Update
    protected override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime) { }
    
    protected override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime) { }

    protected override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        //base.OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
        
        if (hitCollider.CompareTag("Player"))
        {
            Debug.Log("Player hit!");

            // Transition to the knockback state
            _isKnockedBack = true;
        }
    }

    #endregion
    
    #region Switch States
    
    public override void CheckSwitchStates() 
    {
        if (Ctx.Motor.GroundingStatus.IsStableOnGround)
        {
            
            
            if (Ctx.BasicAttackRequested)
            {
                switch (Ctx.character)
                {
                    case Druid:
                        SwitchSubState(_subStateBasicAttack_Druid);
                        break;
                    case Smith:
                        SwitchSubState(_subStateBasicAttack_Smith);
                        break;
                    case CatWoman:
                        SwitchSubState(_subStateBasicAttack_CatWoman);
                        break;
                }

                _isAttacking = true;
            }
            if (Ctx.Special1Requested)
            {
                switch (Ctx.character)
                {
                    case Druid:
                        SwitchSubState(_subStateAttackSpecial1_Druid);
                        break;
                    case Smith:
                        SwitchSubState(_subStateAttackSpecial1_Smith);
                        break;
                    case CatWoman:
                        SwitchSubState(_subStateAttackSpecial1_CatWoman);
                        break;
                }

                _isAttacking = true;
            }
            if (Ctx.Special2Requested) 
            {
                switch (Ctx.character)
                {
                    case Druid:
                        SwitchSubState(_subStateAttackSpecial2_Druid);
                        break;
                    case Smith:
                        SwitchSubState(_subStateAttackSpecial2_Smith);
                        break;
                    case CatWoman:
                        SwitchSubState(_subStateAttackSpecial2_CatWoman);
                        break;
                }

                _isAttacking = true;
            }

            if (_isAttacking)
            {
                AnimatorStateInfo __animStateInfo = Ctx.Anims.GetCurrentAnimatorStateInfo(layerIndex: 0);
                F32 animPercentage = __animStateInfo.normalizedTime;
                
                if (animPercentage > 1.0f)
                {
                    _isAttacking = false;
                }
            }
            
            if (!_isAttacking)
            {
                if (Ctx.IsMovementPressed )
                {
                    SwitchSubState(_subStateWalk);
                }
                else //if (!Ctx.IsMovementPressed)
                {
                    SwitchSubState(_subStateIdle);
                }
            }

            if (_isKnockedBack)
            {
                SwitchState(Factory.KnockBack());
            }
            
            // if player is grounded and jump is pressed , switch to jump state
            if (Ctx.JumpRequested) //&& !Ctx.RequireNewJumpPress)
            {
                // Check if we're actually are allowed to jump
                if (!Ctx.JumpConsumed && Ctx.CanJumpAgain)
                {
                    SwitchState(Factory.Jump());
                }
            }
           
        }
        else
        {
            SwitchState(Factory.Air());
        }
    }

    #endregion
    
}
