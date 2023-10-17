using UnityEngine;

using static UnityEngine.Mathf;

public sealed class PlayerRisingState : PlayerBaseState
{
    public PlayerRisingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) { }

    private const float MAX_FALL_SPEED = 20.0f;

    private float _currentYVelocity;

    public override void EnterState() 
    {
        Debug.Log("Entering Rising State");
        _currentYVelocity = Ctx.Motor.BaseVelocity.y; //Should be 0 when entering falling state but just in case
    }
    public override void ExitState()
    {
        Debug.Log("Exiting Rising State");
    }
    
    public override void UpdateState(ref Vector3 currentVelocity, float deltaTime) 
    {
        //Use Verlet integration to calculate the new velocity
        float __previousYVelocity = _currentYVelocity;
        _currentYVelocity += (Ctx.Gravity * Time.deltaTime);
        currentVelocity.y = Max((__previousYVelocity + _currentYVelocity) * 0.5f, -MAX_FALL_SPEED);
    }
    
    public override void CheckSwitchStates() 
    {
        // if (Ctx.IsFalling)
        // {
        //     SwitchState(Factory.Falling());
        // }
    }
    
}