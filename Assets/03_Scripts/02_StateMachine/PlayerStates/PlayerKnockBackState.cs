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

    [SerializeField] private F32   knockbackForce    = 100.0f;
    [SerializeField] private F32x3 knockbackVector   = new(x: 0.0f, y: 1.0f, z: -1.0f);
    [SerializeField] private F32   knockbackDuration = 1.0f;

    //[SerializeField] private F32 upwardForce = 5.0f; 

    private F32x3 _knockbackDirection;
    private F32   _knockbackTimer;
    //private Bool  _hasKnockedback = false;

    #endregion

    #region Constructor

    public PlayerKnockbackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext: currentContext, playerStateFactory: playerStateFactory)
    { 
        IsRootState = true;
    }

    #endregion

    #region Enter/Exit

    public override void EnterState()
    {
        //  knockback animation or trigger other effects here
        //_knockbackDirection = CalculateKnockbackDirection();
        
        //Rotate knockback vector to match player orientation
        F32x3 __rotatedKnockbackVector = Ctx.transform.rotation * (Vector3)knockbackVector;
        
        _knockbackDirection = normalizesafe(__rotatedKnockbackVector);
        _knockbackTimer     = 0.0f;
        //_hasKnockedback     = false;
        Debug.Log(message: $"Has entered Knockback - frame ({Time.frameCount})");
    }

    public override void ExitState()
    {
        //_hasKnockedback = false;
        
        Debug.Log(message: $"Has exited Knockback - frame ({Time.frameCount})");
    }

    #endregion

    #region Update
    
    private Bool NotOnGround => Ctx.Motor.GroundingStatus.FoundAnyGround == false;

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (_knockbackTimer == 0)
        {
            // Force unground to allow for knockback
            Ctx.Motor.ForceUnground();
            
            // Apply knockback force (once)
            currentVelocity += (Vector3)(_knockbackDirection * knockbackForce * deltaTime);
        }
        
        _knockbackTimer += deltaTime;
        
        
        // F32x3 __knockbackDirection = Ctx.Motor.CharacterUp;
        //
        // if (Ctx.Motor.GroundingStatus is
        //     {
        //         FoundAnyGround:   true, 
        //         IsStableOnGround: false,
        //     })
        // {
        //     __knockbackDirection = Ctx.Motor.GroundingStatus.GroundNormal;
        // }
        //
        // if (_knockbackTimer < knockbackDuration)
        // {
        //     currentVelocity += (Vector3)(__knockbackDirection * knockbackForce * deltaTime);
        //     currentVelocity += Vector3.up + Vector3.back * upwardForce * deltaTime;
        //     _knockbackTimer += deltaTime;
        //     _hasKnockedback = true;
        // }
        // else
        // {
        //     // Exit knockback state when the duration is over
        //     Ctx.Motor.ForceUnground();
        //     
        // }
    }

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {

    }

    #endregion

    #region Switch States

    public override void CheckSwitchStates()
    {
        // if(_hasKnockedback)
        // {
        //     SwitchState(newState: Factory.Air());
        // }
        
        
        if (_knockbackTimer >= knockbackDuration)
        {
            SwitchState(newState: Factory.Air());
        }
    }

    #endregion
    // private F32x3 CalculateKnockbackDirection()
    // {
    //     return Vector3.forward; 
    // }
}
