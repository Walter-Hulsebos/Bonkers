using System;

using KinematicCharacterController;

using UnityEngine;

using F32  = System.Single;
using Bool = System.Boolean;

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

    public virtual void EnterState()
    {
    }
    
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
        currentSubState?.UpdateVelocity(ref currentVelocity, deltaTime);
        UpdateVelocity(ref currentVelocity, deltaTime);
    }
    public void UpdateRotations(ref Quaternion currentRotation, F32 deltaTime)
    {
        currentSubState?.UpdateRotations(ref currentRotation, deltaTime);
        UpdateRotation(ref currentRotation, deltaTime);
    }
    
    protected abstract void UpdateVelocity(ref Vector3    currentVelocity, F32 deltaTime);
    protected abstract void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime);
    
    public void UpdateBeforeCharacterUpdate(F32 deltaTime)
    {
        currentSubState?.UpdateBeforeCharacterUpdate(deltaTime);
        BeforeCharacterUpdate(deltaTime);
    }
    public void UpdatePostGroundingUpdate(F32 deltaTime)
    {
        currentSubState?.UpdatePostGroundingUpdate(deltaTime);
        PostGroundingUpdate(deltaTime);
    }
    public void UpdateAfterCharacterUpdate(F32 deltaTime)
    {
        currentSubState?.UpdateAfterCharacterUpdate(deltaTime);
        AfterCharacterUpdate(deltaTime);
    }
    public void UpdateOnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        currentSubState?.UpdateOnGroundHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
        OnGroundHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
    }
    
    public void UpdateOnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        currentSubState?.UpdateOnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
        OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
    }
    
    /// <summary>
    /// This is called before the motor does anything
    /// </summary>
    protected virtual void BeforeCharacterUpdate(F32 deltaTime){}
    /// <summary>
    /// This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
    /// </summary>
    protected virtual void PostGroundingUpdate(F32 deltaTime){}
    /// <summary>
    /// This is called after the motor has finished everything in its update
    /// </summary>
    protected virtual void AfterCharacterUpdate(F32 deltaTime){}
    
    /// <summary>
    /// This is called when the motor's ground probing detects a ground hit
    /// </summary>
    protected virtual void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}
    /// <summary>
    /// This is called when the motor's movement logic detects a hit
    /// </summary>
    protected virtual void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}
    
    
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
