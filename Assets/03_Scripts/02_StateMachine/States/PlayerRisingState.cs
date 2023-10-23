using UnityEngine;

using static UnityEngine.Mathf;
using static Unity.Mathematics.math;

public sealed class PlayerRisingState : PlayerBaseState
{
    
    #region Variables
    
    private const float MAX_FALL_SPEED = 20.0f;

    private float _currentYVelocity;
    
    #endregion
    
    #region Constructor
    
    public PlayerRisingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) { }
    
    #endregion

    #region Enter/Exit
    
    public override void EnterState() 
    {
        //Debug.Log("Entering Rising State");
        _currentYVelocity = Ctx.Motor.BaseVelocity.y; //Should be 0 when entering falling state but just in case
    }
    public override void ExitState()
    {
        //Debug.Log("Exiting Rising State");
    }
    
    #endregion
    
    #region Update
    
    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) 
    {
        //Use Verlet integration to calculate the new velocity
        //TODO: Evaluate if using _currentYVelocity is correct here, since it isn't shared with the Falling state.
        float __previousYVelocity = _currentYVelocity;
        _currentYVelocity += (length(Ctx.Gravity) * Time.deltaTime);
        currentVelocity.y = max((__previousYVelocity + _currentYVelocity) * 0.5f, -MAX_FALL_SPEED);
    }
    
    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }
    
    #endregion
    
}