using System;

using UnityEngine;

using static UnityEngine.Mathf;

public sealed class PlayerFallingState : PlayerBaseState
{
    
    #region Variables

    private const float MAX_FALL_SPEED = 20.0f;

    private float _currentYVelocity;

    #endregion

    #region Constructor
    
    public PlayerFallingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    
    #endregion

    #region Enter/Exit

    public override void EnterState() 
    {
        //Debug.Log("Entering Falling State");
        _currentYVelocity = Ctx.Motor.BaseVelocity.y; //Should be 0 when entering falling state but just in case
    }

    public override void ExitState()
    {
        //Debug.Log("Exiting Falling State");
    }
    
    #endregion
    
    #region Update
    
    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) 
    {
        // const Single FALL_MULTIPLIER = 2f;
        //
        // //Use Verlet integration to calculate the new velocity
        // float __previousYVelocity = _currentYVelocity;
        // _currentYVelocity += (Ctx.Gravity * FALL_MULTIPLIER * Time.deltaTime);
        // //Max((__previousYVelocity + _currentYVelocity) * 0.5f, -MAX_FALL_SPEED);
        // currentVelocity.y = Min((__previousYVelocity + _currentYVelocity) * 0.5f, MAX_FALL_SPEED);
        
        //TODO: Replace this temp gravity with the real verlet integration based gravity.
        currentVelocity.y -= 30 * deltaTime;
    }
    
    protected override void UpdateRotation(ref Quaternion currentRotation, Single deltaTime) { }
    
    #endregion
    
}