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

    public PlayerBaseState KnockBack() 
    {
        return new PlayerKnockbackState(currentContext: context, playerStateFactory: this);
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
    public PlayerBaseState DruidBasicAttack()
    {
        return new D_BasicAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState DruidSpecial1()
    {
        return new D_S1AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState DruidSpecial2()
    {
        return new D_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    #endregion

    #region Smith Attack
    public PlayerBaseState SmithBasicAttack()
    {
        return new S_BasicAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState SmithSpecial1()
    {
        return new S_S1AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState SmithSpecial2()
    {
        return new S_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    #endregion

    #region CatWoman
    public PlayerBaseState CatWomanBasicAttack()
    {
        return new C_BasicAttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState CatWomanSpecial1()
    {
        return new C_S1AttackState(currentContext: context, playerStateFactory: this);
    }
    public PlayerBaseState CatWomanSpecial2()
    {
        return new C_S2AttackState(currentContext: context, playerStateFactory: this);
    }
    #endregion
}
