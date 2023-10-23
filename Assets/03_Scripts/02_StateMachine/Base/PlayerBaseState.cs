using System;

using UnityEngine;

using F32 = System.Single;

[Serializable]
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
    
    //protected bool canUpdate = true;

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    {
        ctx = currentContext;
        factory = playerStateFactory;
    }

    public abstract void EnterState();
    public virtual void ExitState()
    {
        //canUpdate = false;
    }
    
    public virtual void CheckSwitchStates() { }
    //public virtual  void CheckSwitchSubStates() { }

    //public virtual void InitialSubState() { }
    

    public void UpdateStates() 
    {
        //if (!canUpdate) return;

        if (currentSubState != null)
        {
            currentSubState.UpdateStates(); 
        }

        CheckSwitchStates();
        //CheckSwitchSubStates();
    }

    public void UpdateVelocities(ref Vector3 currentVelocity, F32 deltaTime)
    {
        //if (!canUpdate) return;

        if (currentSubState != null)
        {
            currentSubState.UpdateVelocity(ref currentVelocity, deltaTime); 
        }
        UpdateVelocity(ref currentVelocity, deltaTime);
    }
    
    public void UpdateRotations(ref Quaternion currentRotation, F32 deltaTime)
    {
        //if (!canUpdate) return;

        if (currentSubState != null)
        {
            currentSubState.UpdateRotations(ref currentRotation, deltaTime); 
        }
        UpdateRotation(ref currentRotation, deltaTime);
    }
    
    protected abstract void UpdateVelocity(ref Vector3    currentVelocity, F32 deltaTime);
    protected abstract void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime);

    protected void SwitchState(PlayerBaseState newState)
    {
        if(newState == this) return;
        
        //current state exits state
        ExitState();
        //this.canUpdate = false;

        // new state enters state
        newState.EnterState();
        //newState.canUpdate = true;

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

    private void SetSuperState(PlayerBaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }
    private void SetSubState(PlayerBaseState newSubState) 
    {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
    
    protected void SwitchSubState(PlayerBaseState newSubState)
    {
        if(newSubState == currentSubState) return;
        
        if (currentSubState != null)
        {
            currentSubState.ExitState();
            //currentSubState.canUpdate = false;   
        }
        
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
        
        currentSubState.EnterState();
        //currentSubState.canUpdate = true;
    }

}
