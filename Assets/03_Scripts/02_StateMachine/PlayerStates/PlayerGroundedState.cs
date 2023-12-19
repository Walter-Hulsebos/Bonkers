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
    private PlayerBaseState _subStateLightAttack_Druid;
    private PlayerBaseState _subStateHeavyAttack_Druid;
    private PlayerBaseState _subStateAttackSpecial1_Druid;
    private PlayerBaseState _subStateAttackSpecial2_Druid;
    private PlayerBaseState _subStateUltimate_Druid;
    #endregion

    #region Smith
    private PlayerBaseState _subStateLightAttack_Smith;
    private PlayerBaseState _subStateHeavyAttack_Smith;
    private PlayerBaseState _subStateAttackSpecial1_Smith;
    private PlayerBaseState _subStateAttackSpecial2_Smith;
    private PlayerBaseState _subStateUltimate_Smith;

    #endregion

    #region CatWoman
    private PlayerBaseState _subStateLightAttack_CatWoman;
    private PlayerBaseState _subStateHeavyAttack_CatWoman;
    private PlayerBaseState _subStateAttackSpecial1_CatWoman;
    private PlayerBaseState _subStateAttackSpecial2_CatWoman;
    private PlayerBaseState _subStateUltimate_CatWoman;

    #endregion

    #region Gabriel
    private PlayerBaseState _subStateLightAttack_Gabriel;
    private PlayerBaseState _subStateHeavyAttack_Gabriel;
    private PlayerBaseState _subStateAttackSpecial1_Gabriel;
    private PlayerBaseState _subStateAttackSpecial2_Gabriel;
    private PlayerBaseState _subStateUltimate_Gabriel;
    #endregion

    #region Roberto
    private PlayerBaseState _subStateLightAttack_Roberto;
    private PlayerBaseState _subStateHeavyAttack_Roberto;
    private PlayerBaseState _subStateAttackSpecial1_Roberto;
    private PlayerBaseState _subStateAttackSpecial2_Roberto;
    private PlayerBaseState _subStateUltimate_Roberto;
    #endregion

    #region Water Girl
    private PlayerBaseState _subStateLightAttack_WaterGirl;
    private PlayerBaseState _subStateHeavyAttack_WaterGirl;
    private PlayerBaseState _subStateAttackSpecial1_WaterGirl;
    private PlayerBaseState _subStateAttackSpecial2_WaterGirl;
    private PlayerBaseState _subStateUltimate_WaterGirl;
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
        _subStateLightAttack_Druid    = Factory.DruidLightAttack();
        _subStateHeavyAttack_Druid = Factory.DruidHeavyAttack();
        _subStateAttackSpecial1_Druid = Factory.DruidSpecial1();
        _subStateAttackSpecial2_Druid = Factory.DruidSpecial2();
        _subStateUltimate_Druid = Factory.DruidUltimate();
        #endregion

        #region Smith Attack
        _subStateLightAttack_Smith =    Factory.SmithLightAttack();
        _subStateHeavyAttack_Smith = Factory.SmithHeavyAttack();
        _subStateAttackSpecial1_Smith = Factory.SmithSpecial1();
        _subStateAttackSpecial2_Smith = Factory.SmithSpecial2();
        _subStateUltimate_Smith = Factory.SmithUltimate();

        #endregion

        #region CatWoman Attack
        _subStateLightAttack_CatWoman = Factory.CatWomanLightAttack();
        _subStateHeavyAttack_CatWoman = Factory.CatWomanHeavyAttack();
        _subStateAttackSpecial1_CatWoman = Factory.CatWomanSpecial1();
        _subStateAttackSpecial2_CatWoman = Factory.CatWomanSpecial2();
        _subStateUltimate_CatWoman = Factory.CatWomanUltimate();

        #endregion

        #region Gabriel
        _subStateLightAttack_Gabriel = Factory.GabrielLightAttack();
        _subStateHeavyAttack_Gabriel = Factory.GabrielHeavyAttack();
        _subStateAttackSpecial1_Gabriel = Factory.GabrielSpecial1();
        _subStateAttackSpecial2_Gabriel = Factory.GabrielSpecial2();
        _subStateUltimate_Gabriel = Factory.GabrielUltimate();
        #endregion

        #region Roberto
        _subStateLightAttack_Roberto = Factory.RobertoLightAttack();
        _subStateHeavyAttack_Roberto = Factory.RobertoHeavyAttack();
        _subStateAttackSpecial1_Roberto = Factory.RobertoSpecial1();
        _subStateAttackSpecial2_Roberto = Factory.RobertoSpecial2();
        _subStateUltimate_Roberto = Factory.RobertoUltimate();
        #endregion

        #region WaterGirl
        _subStateLightAttack_WaterGirl = Factory.WaterGirlLightAttack();
        _subStateHeavyAttack_WaterGirl = Factory.WaterGirlHeavyAttack();
        _subStateAttackSpecial1_WaterGirl = Factory.WaterGirlSpecial1();
        _subStateAttackSpecial2_WaterGirl = Factory.WaterGirlSpecial2();
        _subStateUltimate_WaterGirl = Factory.WaterGirlUltimate();
        #endregion
    }

    #endregion

    #region Enter/Exit

    public override void EnterState()
    {
        Debug.Log($"Entering Grounded State - frame ({Time.frameCount})", context: Ctx);
        
        _isKnockedBack = false;
        Ctx.DoubleJumpAvailable = true;
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
            
            
            if (Ctx.LightAttackRequested)
            {
                switch (Ctx.character)
                {
                    case Druid:
                        SwitchSubState(_subStateLightAttack_Druid);
                        break;
                    case Smith:
                        SwitchSubState(_subStateLightAttack_Smith);
                        break;
                    case CatWoman:
                        SwitchSubState(_subStateLightAttack_CatWoman);
                        break;
                    case Gabriel:
                        SwitchSubState(_subStateLightAttack_Gabriel);
                        break;
                    case Roberto:
                        SwitchSubState(_subStateLightAttack_Roberto);
                        break;
                    case WaterGirl:
                        SwitchSubState(_subStateLightAttack_WaterGirl);
                        break;
                }

                _isAttacking = true;
            }
            if (Ctx.HeavyAttackRequested)
            {
                switch (Ctx.character)
                {
                    case Druid:
                        SwitchSubState(_subStateHeavyAttack_Druid);
                        break;
                    case Smith:
                        SwitchSubState(_subStateHeavyAttack_Smith);
                        break;
                    case CatWoman:
                        SwitchSubState(_subStateHeavyAttack_CatWoman);
                        break;
                    case Gabriel:
                        SwitchSubState(_subStateHeavyAttack_Gabriel);
                        break;
                    case Roberto:
                        SwitchSubState(_subStateHeavyAttack_Roberto);
                        break;
                    case WaterGirl:
                        SwitchSubState(_subStateHeavyAttack_WaterGirl);
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
                    case Gabriel:
                        SwitchSubState(_subStateAttackSpecial1_Gabriel);
                        break;
                    case Roberto:
                        SwitchSubState(_subStateAttackSpecial1_Roberto);
                        break;
                    case WaterGirl:
                        SwitchSubState(_subStateAttackSpecial1_WaterGirl);
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
                    case Gabriel:
                        SwitchSubState(_subStateAttackSpecial2_Gabriel);
                        break;
                    case Roberto:
                        SwitchSubState(_subStateAttackSpecial2_Roberto);
                        break;
                    case WaterGirl:
                        SwitchSubState(_subStateAttackSpecial2_WaterGirl);
                        break;
                }

                _isAttacking = true;
            }
            if (Ctx.UltimateRequested)
            {
                switch (Ctx.character)
                {
                    case Druid:
                        SwitchSubState(_subStateUltimate_Druid);
                        break;
                    case Smith:
                        SwitchSubState(_subStateUltimate_Smith);
                        break;
                    case CatWoman:
                        SwitchSubState(_subStateUltimate_CatWoman);
                        break;
                    case Gabriel:
                        SwitchSubState(_subStateUltimate_Gabriel);
                        break;
                    case Roberto:
                        SwitchSubState(_subStateUltimate_Roberto);
                        break;
                    case WaterGirl:
                        SwitchSubState(_subStateUltimate_WaterGirl);
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
