using UnityEngine;

public abstract class PlayerBaseState
{
    private bool isRootState = false;
    private PlayerStateMachine ctx;
    private PlayerStateFactory factory;
    private PlayerBaseState currentSubState;
    private PlayerBaseState currentSuperState;

    protected bool IsRootState { set { isRootState = value; }}
    protected PlayerStateMachine Ctx { get { return ctx; }}
    protected PlayerStateFactory Factory { get { return factory; }}

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    {
        ctx = currentContext;
        factory = playerStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState(ref Vector3 currentVelocity, float deltaTime);
    public abstract void ExitState();
    
    public virtual void CheckSwitchSubStates() { }
    public abstract void CheckSwitchStates();

    //public virtual void InitialSubState() { }
    
    public void UpdateStates(ref Vector3 currentVelocity, float deltaTime) 
    {
        CheckSwitchSubStates();
        CheckSwitchStates();
        
        UpdateState(ref currentVelocity, deltaTime);
        if(currentSubState != null)
        {
            currentSubState.UpdateStates(ref currentVelocity, deltaTime);
        }
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        //current state exits state
        ExitState();

        // new state enters state
        newState.EnterState();

        if (isRootState)
        {
            //switch current state of context
            ctx.CurrentState = newState;
        }
        else if (currentSuperState != null)
        {
            currentSuperState.SetSubState(newState);
        }

    }

    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState) 
    {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

}
