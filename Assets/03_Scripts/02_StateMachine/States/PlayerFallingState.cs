using System;

using UnityEngine;

using static UnityEngine.Mathf;

public sealed class PlayerFallingState : PlayerBaseState
{
    public PlayerFallingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    private const float MAX_FALL_SPEED = 20.0f;

    private float _currentYVelocity;

    public override void EnterState() 
    {
        Debug.Log("Entering Falling State");
        _currentYVelocity = Ctx.Motor.BaseVelocity.y; //Should be 0 when entering falling state but just in case
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Falling State");
    }
    
    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) 
    {
        const Single FALL_MULTIPLIER = 2f;

        //Use Verlet integration to calculate the new velocity
        float __previousYVelocity = _currentYVelocity;
        _currentYVelocity += (Ctx.Gravity * FALL_MULTIPLIER * Time.deltaTime);
        currentVelocity.y =  Max((__previousYVelocity + _currentYVelocity) * 0.5f, -MAX_FALL_SPEED);
    }
    
    protected override void UpdateRotation(ref Quaternion currentRotation, Single deltaTime) { }
}