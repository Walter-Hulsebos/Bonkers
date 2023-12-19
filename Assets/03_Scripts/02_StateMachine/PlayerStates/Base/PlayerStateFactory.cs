using Bonkers._02_StateMachine.States;

public class PlayerStateFactory
{
    private PlayerStateMachine context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        context = currentContext;
    }
    
    public PlayerBaseState Grounded() 
    {
        return new PlayerGroundedState(currentContext: context, playerStateFactory: this);
    }
        public PlayerBaseState Idle()
        {
            return new PlayerIdleState(currentContext: context, playerStateFactory: this); 
        }
        public PlayerBaseState Walk()
        {
            return new PlayerWalkState(currentContext: context, playerStateFactory: this);
        }
        
    public PlayerBaseState Jump() 
    {
        return new PlayerJumpState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState WallJump()
    {
        return new PlayerWallJumpState(currentContext: context, playerStateFactory: this);
    }

    public PlayerBaseState KnockBack() 
    {
        return new PlayerKnockbackState(currentContext: context, playerStateFactory: this);
    }

    public PlayerBaseState ExtraJump()
    {
        return new PlayerExtraJumpState(currentContext: context, playerStateFactory: this);
    }

    public PlayerBaseState WallSliding()
    {
        return new PlayerWallSlideState(currentContext: context, playerStateFactory: this);
    }

    public PlayerBaseState Air() 
    {
        return new PlayerAirState(currentContext: context, playerStateFactory: this);
    }
        public PlayerBaseState Falling() 
        {
            return new PlayerFallingState(currentContext: context, playerStateFactory: this);
        }
        public PlayerBaseState Rising() 
        {
            return new PlayerRisingState(currentContext: context, playerStateFactory: this);
        }

    #region Druid Attack
    public PlayerBaseState DruidLightAttack()
    {
        return new D_LightAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState DruidHeavyAttack()
    {
        return new D_HeavyAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState DruidSpecial1()
    {
        return new D_S1AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState DruidSpecial2()
    {
        return new D_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState DruidUltimate()
    {
        return new D_UltimateState(currentContext: context, playerStateFactory: this);
    }
    #endregion

    #region Smith Attack
    public PlayerBaseState SmithLightAttack()
    {
        return new S_LightAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState SmithHeavyAttack()
    {
        return new S_HeavyAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState SmithSpecial1()
    {
        return new S_S1AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState SmithSpecial2()
    {
        return new S_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState SmithUltimate()
    {
        return new S_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    #endregion

    #region CatWoman
    public PlayerBaseState CatWomanLightAttack()
    {
        return new C_LightAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState CatWomanHeavyAttack()
    {
        return new C_HeavyAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState CatWomanSpecial1()
    {
        return new C_S1AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState CatWomanSpecial2()
    {
        return new C_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState CatWomanUltimate()
    {
        return new C_UltimateState(currentContext: context, playerStateFactory: this);
    }
    #endregion

    #region Gabriel
    public PlayerBaseState GabrielLightAttack()
    {
        return new G_LightAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState GabrielHeavyAttack()
    {
        return new G_HeavyAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState GabrielSpecial1()
    {
        return new G_S1AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState GabrielSpecial2()
    {
        return new G_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState GabrielUltimate()
    {
        return new G_UltimateState(currentContext: context, playerStateFactory: this);
    }
    #endregion

    #region Roberto
    public PlayerBaseState RobertoLightAttack()
    {
        return new R_LightAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState RobertoHeavyAttack()
    {
        return new R_HeavyAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState RobertoSpecial1()
    {
        return new R_S1AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState RobertoSpecial2()
    {
        return new R_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState RobertoUltimate()
    {
        return new R_UltimateState(currentContext: context, playerStateFactory: this);
    }
    #endregion

    #region WaterGirl
    public PlayerBaseState WaterGirlLightAttack()
    {
        return new W_LightAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState WaterGirlHeavyAttack()
    {
        return new W_HeavyAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState WaterGirlSpecial1()
    {
        return new W_S1AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState WaterGirlSpecial2()
    {
        return new W_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState WaterGirlUltimate()
    {
        return new W_UltimateState(currentContext: context, playerStateFactory: this);
    }
    #endregion
}
