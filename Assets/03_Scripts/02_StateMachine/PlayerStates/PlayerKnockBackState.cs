using System;
using UnityEngine;

using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Bool  = System.Boolean;

[Serializable]
public sealed class PlayerKnockbackState : PlayerBaseState
{
    #region Variables

    [SerializeField] private F32 knockbackForce = 10.0f;
    [SerializeField] private F32 knockbackDuration = 1.0f;

    [SerializeField] private F32 upwardForce = 5.0f; 

    private F32x3 _knockbackDirection;
    private F32   _knockbackTimer;

    #endregion

    #region Constructor

    public PlayerKnockbackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = false;
    }

    #endregion

    #region Enter/Exit

    public override void EnterState()
    {
        //  knockback animation or trigger other effects here
        _knockbackDirection = CalculateKnockbackDirection(); 
        _knockbackTimer = 0.0f;
    }

    public override void ExitState()
    {

    }

    #endregion

    #region Update

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // Apply knockbackfor a certain duration
        if (_knockbackTimer < 1.0f)
        {
            currentVelocity += (Vector3)(_knockbackDirection * knockbackForce * deltaTime);
            currentVelocity += Vector3.up * upwardForce * deltaTime; // Add upward force
            _knockbackTimer += deltaTime;
            SwitchState(Factory.Air());
        }
        else
        {
            // Exit knockback state when the duration is over
            
        }
    }

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {

    }

    #endregion

    #region Switch States

    public override void CheckSwitchStates()
    {
        
    }

    #endregion
    private F32x3 CalculateKnockbackDirection()
    {
        return Vector3.forward; 
    }
}
