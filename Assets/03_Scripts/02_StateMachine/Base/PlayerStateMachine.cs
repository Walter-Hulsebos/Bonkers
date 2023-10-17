using System;
using System.Linq;

using Bonkers.Characters;

using CGTK.Utils.Extensions.Math.Math;
using CGTK.Utils.UnityFunc;

using UnityEngine;
using static UnityEngine.Mathf;
    
using Unity.Netcode;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

using KinematicCharacterController;

using UnityEngine.Serialization;

using static Bonkers.Characters.OrientationMethod;

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;
    
using I32   = System.Int32;
using I32x2 = Unity.Mathematics.int2;
using I32x3 = Unity.Mathematics.int3;
    
using Bool  = System.Boolean;
using Rotor = Unity.Mathematics.quaternion;

public class PlayerStateMachine : MonoBehaviour, ICharacterController
{
    #region References

    [field:SerializeField] public KinematicCharacterMotor Motor { get; private set; }
    [field:SerializeField] public Camera                  Cam   { get; private set; }
    [field:SerializeField] public Animator                Anims { get; private set; }

    #endregion

    #region Inputs

    [SerializeField] private OrientationMethod orientationMethod = TowardsMovement;
    
    [SerializeField] private UnityFunc<F32x2> getMoveInput;
    [SerializeField] private UnityFunc<Bool>  getJumpInput;
    
    public F32x3 MoveInputVector { get; private set; }
    public F32x3 LookInputVector { get; private set; }

    #endregion
    
    #region Animator Hashes

    public I32 WalkHash { get; private set; }
    public I32 JumpHash { get; private set; }
    public I32 IdleHash { get; private set; }

    #endregion
    


    //animator hashes
    // private I32 isWalkingHash;
    //
    // private I32 isJumpingHash;
    //I32 isRunningHash;

    //input values
    private Vector2 currentMovementInput;

    // [ReadOnly]
    // [SerializeField] private Vector3 currentMovement;
    // //Vector3 currentRunMovement;
    // [ReadOnly]
    // [SerializeField] private Vector3 appliedMovement;
    
    public F32x3 CameraPlanarDir { get; private set; }
    public Rotor CameraPlanarRot { get; private set; }

    private Bool isMovementPressed;
    //Bool isRunPressed;

    //const
    private const F32 rotationFactorPerFrame = 15.0f;
    private const I32 zero                   = 0;
    private       F32 gravity                = -9.8f;
    private       F32 groundedGravity        = -.05f;

    //moving
    public F32 moveGroundMaxSpeed  =  4.0f;
    public F32 moveGroundSharpness = 15.0f;
    //air
    public F32 moveAirMaxSpeed = 3.0f;
    public F32 moveAirAccSpeed = 5.0f;
    //public F32 runMultiplier = 8.0f;

    //jumping
    private Bool isJumpPressed       = false;
    private Bool isJumping           = false;
    private Bool requireNewJumpPress = false;
    private F32  initialJumpVelocity;


    // state variables
    private PlayerBaseState    currentState;
    private PlayerStateFactory states;

    // getters and setters
    public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }
    
    public Animator Animator { get { return Anims; } }
    //public CharacterController Motor { get { return motor; } }
    // public I32 IsJumpingHash { get { return isJumpingHash; } }
    // public I32 IsIdleHash    { get { return isWalkingHash; } }
    // public I32 IsWalkingHash { get { return isWalkingHash; } }
    
    public Bool RequireNewJumpPress {get { return requireNewJumpPress; } set {requireNewJumpPress = value; } }
    public Bool IsJumping { set {isJumping = value; } }
    public Bool IsFalling => (Motor.BaseVelocity.y <= 0.0f) && !Motor.GroundingStatus.IsStableOnGround;
    public Bool IsRising  => (Motor.BaseVelocity.y >  0.0f) && !Motor.GroundingStatus.IsStableOnGround;
    public Bool IsJumpPressed {  get { return isJumpPressed; } }
    public Bool IsMovementPressed => lengthsq(getMoveInput.Invoke()) > 0.0f;
    //public Bool IsRunPressed { get { return isRunPressed; } }
    public F32 MoveGroundMaxSpeed { get { return moveGroundMaxSpeed; } }
    //public F32 RunMultiplier { get { return runMultiplier; } }
    public F32 InitialJumpVelocity { get { return initialJumpVelocity; } }
    public F32 GroundedGravity     { get { return groundedGravity; } }
    public F32 Gravity             { get ; set; }
    
    // public F32 CurrentMovementY { get { return currentMovement.y; } set { currentMovement.y = value; } }
    // public F32 AppliedMovementY { get { return appliedMovement.y; } set { appliedMovement.y = value; } }
    // public F32 AppliedMovementX { get { return appliedMovement.x; } set { appliedMovement.x = value; } }
    // public F32 AppliedMovementZ { get { return appliedMovement.z; } set { appliedMovement.z = value; } }
    //public Vector2 CurrentMovementInput { get { return currentMovementInput; } }
    
    //public Vector3 CurrentVelocity { get; private set; }


    #if UNITY_EDITOR
    private void Reset()
    {
        Motor = GetComponent<KinematicCharacterMotor>();
        Anims = GetComponent<Animator>();
        
        Cam   = GetComponent<Camera>();
        Cam   = transform.parent.GetComponentInChildren<Camera>();
    }
    #endif

    private void Awake()
    {
        //set reference variables
        //playerInput = new PlayerInput();
        //motor = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();

        // setup state
        states = new PlayerStateFactory(currentContext: this);

        //set hash references
        // isWalkingHash = Animator.StringToHash(name: "isWalking");
        // isJumpingHash = Animator.StringToHash(name: "isJumping");
        //isRunningHash = Animator.StringToHash(name: "isRunning");
        
        WalkHash = Animator.StringToHash(name: "walk");
        JumpHash = Animator.StringToHash(name: "jump");
        IdleHash = Animator.StringToHash(name: "idle");
        
        //SetupJumpVariables();
    }

    // private void SetupJumpVariables()
    // {
    //     F32 timeToApex = maxJumpTime * 0.5f;
    //     gravity             = (-2    * maxJumpHeight) / pow(timeToApex, 2);
    //     initialJumpVelocity = (+2    * maxJumpHeight) / timeToApex;
    // }

    private void OnEnable()
    {
        // Assign to motor
        Motor.CharacterController = this;
    }

    private void Start() 
    { 
        currentState = states.Grounded();
        currentState.EnterState();
     }

    // Update is called once per frame
    private void Update()
    {
        UpdateInputs();
        //HandleRotation();
        //currentState.UpdateStates();
        
        //Motor.MoveCharacter(toPosition: appliedMovement * Time.deltaTime);
        //motor.Move(appliedMovement * Time.deltaTime);
    }

    private void UpdateInputs()
    {
        //if(!Application.isFocused) return;
            
        F32x2 __moveAxis = getMoveInput.Invoke();

        F32x3 __moveInputVector = Vector3.ClampMagnitude(vector: new F32x3(x: __moveAxis.x, y: 0f, z: __moveAxis.y), maxLength: 1f);

        Rotor __cameraRotation = Cam.transform.rotation;
            
        CameraPlanarDir = normalize(Vector3.ProjectOnPlane(vector: mul(__cameraRotation, forward()), planeNormal: Motor.CharacterUp));
            
        if (lengthsq(CameraPlanarDir) == 0f)
        {
            CameraPlanarDir = normalize(Vector3.ProjectOnPlane(vector: mul(__cameraRotation, up()), planeNormal: Motor.CharacterUp));
        }
        
        CameraPlanarRot = Rotor.LookRotation(forward: CameraPlanarDir, up: Motor.CharacterUp);

        // Move and look inputs
        MoveInputVector = mul(CameraPlanarRot, __moveInputVector);

        LookInputVector = CameraPlanarDir;

        // LookInputVector = orientationMethod switch
        // {
        //     TowardsCamera   => CameraPlanarDir,
        //     TowardsMovement => normalize(MoveInputVector),
        //     _               => LookInputVector,
        // };

        // // Jumping input
        // if (getJumpInput.Invoke())
        // {
        //     _timeSinceJumpRequested = 0f;
        //     _jumpRequested          = true;
        // }
    }
    
    public void BeforeCharacterUpdate(F32 deltaTime)
    {
        // This is called before the motor does anything
    }
    
    /// <summary> Called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling </summary>
    public void PostGroundingUpdate(F32 deltaTime)
    {
        currentState.UpdateStates();
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
    {
        //currentState.UpdateStates(ref currentVelocity, deltaTime);
        currentState.UpdateVelocities(ref currentVelocity, deltaTime);
    }
    
    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its rotation should be right now. 
    /// This is the ONLY place where you should set the character's rotation
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime)
    {
        //if(!Application.isFocused) return;
        //if(CantMove) return;
        
        currentState.UpdateRotations(ref currentRotation, deltaTime);
        
        //Gravity orientation
        // F32x3   __currentUp            = mul(currentRotation, up());
        // F32x3   __normalizedNegGravity = -normalizesafe(gravity);
        // F32     __orientationSpeed     = 1 - exp(-bonusOrientationSharpness * deltaTime);
        // Vector3 __smoothedGravityDir   = slerpsafe(__currentUp, __normalizedNegGravity, t: __orientationSpeed);
        //
        // currentRotation = Quaternion.FromToRotation(fromDirection: __currentUp, toDirection: __smoothedGravityDir) * currentRotation;
    }

    
    // private void HandleRotation()
    // {
    //     Vector3 positionToLookAt;
    //
    //     positionToLookAt.x = currentMovement.x;
    //     positionToLookAt.y = zero;
    //     positionToLookAt.z = currentMovement.z;
    //     
    //     positionToLookAt.Normalize();
    //
    //     Quaternion currentRotation = transform.rotation;
    //
    //     if (isMovementPressed)
    //     {
    //         Quaternion targetRotation = Quaternion.LookRotation(forward: positionToLookAt);
    //         transform.rotation = Quaternion.Slerp(a: currentRotation, b: targetRotation, t: rotationFactorPerFrame * Time.deltaTime);
    //     }
    // }
    
    // private void OnEnable()
    // {
    //     playerInput.CharacterControls.Enable();
    // }
    //
    // private void OnDisable()
    // {
    //     playerInput.CharacterControls.Disable();
    //
    // }



    // Vector3 __targetMovementVelocity = Vector3.zero;
    // // This is called when the motor wants to know what its velocity should be right now
    // if (Motor.GroundingStatus.IsStableOnGround)
    // {
    //     // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
    //     currentVelocity = Motor.GetDirectionTangentToSurface(direction: currentVelocity, surfaceNormal: Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;
    //
    //     // Calculate target velocity
    //     Vector3 inputRight      = Vector3.Cross(lhs: currentMovementInput, rhs: Motor.CharacterUp);
    //     Vector3 reorientedInput = Vector3.Cross(lhs: Motor.GroundingStatus.GroundNormal, rhs: inputRight).normalized * currentMovementInput.magnitude;
    //     __targetMovementVelocity = reorientedInput * moveGroundMaxSpeed;
    //
    //     // Smooth movement Velocity
    //     currentVelocity = Vector3.Lerp(a: currentVelocity, b: __targetMovementVelocity, t: 1 - Exp(power: -moveGroundSharpness * deltaTime));
    // }
    // else
    // {
    //     // Add move input
    //     if (currentMovementInput.sqrMagnitude > 0f)
    //     {
    //         __targetMovementVelocity = (currentMovementInput * moveAirMaxSpeed);
    //
    //         // Prevent climbing on un-stable slopes with air movement
    //         if (Motor.GroundingStatus.FoundAnyGround)
    //         {
    //             Vector3 perpenticularObstructionNormal = Vector3.Cross(lhs: Vector3.Cross(lhs: Motor.CharacterUp, rhs: Motor.GroundingStatus.GroundNormal), rhs: Motor.CharacterUp).normalized;
    //             __targetMovementVelocity = Vector3.ProjectOnPlane(vector: __targetMovementVelocity, planeNormal: perpenticularObstructionNormal);
    //         }
    //
    //         Vector3 velocityDiff = Vector3.ProjectOnPlane(vector: __targetMovementVelocity - currentVelocity, planeNormal: Vector3.up * Gravity);
    //         currentVelocity += velocityDiff * (moveAirAccSpeed * deltaTime);
    //     }
    //
    //     // Gravity
    //     //currentVelocity += Gravity * deltaTime;
    //
    //     // Drag
    //     //currentVelocity *= (1f / (1f + (Drag * deltaTime)));
    // }

    public void AfterCharacterUpdate(F32 deltaTime)
    {
        // This is called after the motor has finished everything in its update
    }

    public Bool IsColliderValidForCollisions(Collider coll)
    {
        // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        // This is called when the motor's ground probing detects a ground hit
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        // This is called when the motor's movement logic detects a hit
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
    }
}
