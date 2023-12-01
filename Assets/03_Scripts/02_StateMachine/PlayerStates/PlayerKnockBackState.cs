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
    private Bool _hasKnockedback = false;

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
        _hasKnockedback = false;
        Debug.Log("Has entered Knockback");
    }

    public override void ExitState()
    {
        _hasKnockedback = false;
    }

    #endregion

    #region Update

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        F32x3 __knockbackDirection = Ctx.Motor.CharacterUp;

        if (Ctx.Motor.GroundingStatus is
            {
                FoundAnyGround:   true, 
                IsStableOnGround: false,
            })
        {
            __knockbackDirection = Ctx.Motor.GroundingStatus.GroundNormal;
        }

        if (_knockbackTimer < 1.0f)
        {
            currentVelocity += (Vector3)(__knockbackDirection * knockbackForce * deltaTime);
            currentVelocity += Vector3.up * upwardForce * deltaTime;
            _knockbackTimer += deltaTime;
            _hasKnockedback = true;
        }
        else
        {
            // Exit knockback state when the duration is over
            Ctx.Motor.ForceUnground();
            
        }
    }

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {

    }

    #endregion

    #region Switch States

    public override void CheckSwitchStates()
    {
        if(_hasKnockedback)
        {
            SwitchState(Factory.Falling());
        }
    }

    #endregion
    private F32x3 CalculateKnockbackDirection()
    {
        return Vector3.forward; 
    }
}
