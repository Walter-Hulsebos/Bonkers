using System;
using UnityEngine;

using F32  = System.Single;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;

[Serializable]
public sealed class PlayerJumpState : PlayerBaseState
{
    #region Variables

    [SerializeField] private F32 maxJumpHeight = 4.0f;
    [SerializeField] private F32 maxJumpTime   = 0.75f;

    private F32 _initialJumpVelocity;
    
    private Boolean _isJumping = false;

    #endregion

    #region Constructor

    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        
        SetupJumpVariables();
    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entering Jump State");
        //Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        Ctx.Animator.SetTrigger(Ctx.JumpHash);
        
        _isJumping = true;
    }
    public override void ExitState()
    {
        Debug.Log("Exiting Jump State");
        //Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        _isJumping = false;
        
        if(Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
    }
    
    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        //TODO: CurrentMovementY???
        currentVelocity = new Vector3(x: currentVelocity.x, y: _initialJumpVelocity, z: currentVelocity.z);
        
        // // Add move input
        // if (currentMovementInput.sqrMagnitude > 0f)
        // {
        //     __targetMovementVelocity = (currentMovementInput * moveAirMaxSpeed);
        //
        //     // Prevent climbing on un-stable slopes with air movement
        //     if (Motor.GroundingStatus.FoundAnyGround)
        //     {
        //         Vector3 perpenticularObstructionNormal = Vector3.Cross(lhs: Vector3.Cross(lhs: Motor.CharacterUp, rhs: Motor.GroundingStatus.GroundNormal), rhs: Motor.CharacterUp).normalized;
        //         __targetMovementVelocity = Vector3.ProjectOnPlane(vector: __targetMovementVelocity, planeNormal: perpenticularObstructionNormal);
        //     }
        //
        //     Vector3 velocityDiff = Vector3.ProjectOnPlane(vector: __targetMovementVelocity - currentVelocity, planeNormal: Vector3.up * Gravity);
        //     currentVelocity += velocityDiff * (moveAirAccSpeed * deltaTime);
        // }
        //
        // // Gravity
        // //currentVelocity += Gravity * deltaTime;
        //
        // // Drag
        // //currentVelocity *= (1f / (1f + (Drag * deltaTime)));
    }
    
    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }
    
    public override void CheckSwitchStates()
    {
        if (_isJumping)
        {
            SwitchState(Factory.Air());
        }
    }

    #endregion

    #region Custom Methods

    private void SetupJumpVariables()
    {
        F32 __timeToApex = maxJumpTime * 0.5f;
        Ctx.Gravity          = (-2    * maxJumpHeight) / pow(__timeToApex, 2);
        _initialJumpVelocity = (+2    * maxJumpHeight) / __timeToApex;
    }

    #endregion

}