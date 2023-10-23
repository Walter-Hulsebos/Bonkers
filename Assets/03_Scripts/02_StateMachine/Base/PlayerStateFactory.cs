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
}
