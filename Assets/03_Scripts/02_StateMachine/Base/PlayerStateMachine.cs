using System;
using System.Linq;

//using Bonkers.Characters;

using CGTK.Utils.Extensions.Math.Math;
using CGTK.Utils.UnityFunc;

using UnityEngine;
using static UnityEngine.Mathf;
    
using Unity.Netcode;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

using KinematicCharacterController;

using Sirenix.OdinInspector;

using UnityEngine.Serialization;


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

    #if ODIN_INSPECTOR
    [field:FoldoutGroup("References")]
    #endif
    [field:SerializeField] public KinematicCharacterMotor Motor { get; private set; }
    
    #if ODIN_INSPECTOR
    [field:FoldoutGroup("References")]
    #endif
    [field:SerializeField] public Camera                  Cam   { get; private set; }
    
    #if ODIN_INSPECTOR
    [field:FoldoutGroup("References")]
    #endif
    [field:SerializeField] public Animator                Anims { get; private set; }

    #endregion

    #region Inputs
    
    [SerializeField] private UnityFunc<F32x2> getMoveInput;
    [SerializeField] private UnityFunc<Bool>  getJumpInput;

    public F32x3 MoveInputVector { get; private set; }
    public F32x3 LookInputVector { get; private set; }
    public Bool  JumpRequested   { get; internal set; }

    #endregion
    
    #region Animator Hashes

    public I32 WalkHash { get; private set; }
    public I32 JumpHash { get; private set; }
    public I32 IdleHash { get; private set; }

    #endregion
    
    [SerializeField] public F32 JumpPreGroundingGraceTime  { get; private set; } = 0.1f;
    [SerializeField] public F32 JumpPostGroundingGraceTime { get; private set; } = 0.1f;
    
    public F32x3 CameraPlanarDir { get; private set; }
    public Rotor CameraPlanarRot { get; private set; }
    
    //private Bool _requireNewJumpPress = false;


    // state variables
    private PlayerStateFactory _states;

    // getters and setters
    public PlayerBaseState CurrentState { get; internal set; }
    
    //public Bool RequireNewJumpPress {get { return _requireNewJumpPress; } set {_requireNewJumpPress = value; } }
    public Bool IsFalling => (Motor.BaseVelocity.y <= 0.0f) && !Motor.GroundingStatus.IsStableOnGround;
    public Bool IsRising  => (Motor.BaseVelocity.y >  0.0f) && !Motor.GroundingStatus.IsStableOnGround;
    public Bool IsMovementPressed => lengthsq(getMoveInput.Invoke()) > 0.0f;
    

    public Bool JumpConsumed            { get; internal set; } = false;
    public Bool JumpedThisPhysicsFrame  { get; internal set; } = false;
    public F32  TimeSinceJumpRequested  { get; internal set; } = Infinity;
    public F32  TimeSinceLastAbleToJump { get; internal set; } = 0f;

    public Bool CanJumpAgain => (TimeSinceLastAbleToJump <= JumpPostGroundingGraceTime);

    public F32x3 Gravity { get ; set; }

    #if UNITY_EDITOR
    protected virtual void Reset()
    {
        Motor = GetComponent<KinematicCharacterMotor>();
        Anims = GetComponent<Animator>();
        
        Cam   = GetComponent<Camera>();
        Cam   = transform.parent.GetComponentInChildren<Camera>();
    }
    #endif

    private void Awake()
    {
        // setup state
        _states = new PlayerStateFactory(currentContext: this);

        //set hash references
        WalkHash = Animator.StringToHash(name: "walk");
        JumpHash = Animator.StringToHash(name: "jump");
        IdleHash = Animator.StringToHash(name: "idle");
        
        Motor.CharacterController = this;
    }

    private void OnEnable()
    {
        // Assign to motor
        //
        
    }

    private void Start() 
    { 
        CurrentState = _states.Grounded();
        CurrentState.EnterState();
     }

    // Update is called once per frame
    protected virtual void Update()
    {
        //if(!Application.isFocused) return;
            
        F32x2 __moveAxis = getMoveInput.Invoke();

        F32x3 __moveInputVector = Vector3.ClampMagnitude(vector: new F32x3(x: __moveAxis.x, y: 0f, z: __moveAxis.y), maxLength: 1f);

        Rotor __cameraRotation = Cam.transform.rotation;
        CameraPlanarDir = normalizesafe(Vector3.ProjectOnPlane(vector: mul(__cameraRotation, forward()), planeNormal: Motor.CharacterUp));
        if (lengthsq(CameraPlanarDir) == 0f)
        {
            CameraPlanarDir = normalizesafe(Vector3.ProjectOnPlane(vector: mul(__cameraRotation, up()), planeNormal: Motor.CharacterUp));
        }
        CameraPlanarRot = Rotor.LookRotation(forward: CameraPlanarDir, up: Motor.CharacterUp);

        // Move and look inputs
        MoveInputVector = mul(CameraPlanarRot, __moveInputVector);

        F32x3 __lookInputNormalized = normalizesafe(MoveInputVector);

        // Only set the look input if we are moving
        //if (any(__lookInputNormalized > 0f))
        if(!lengthsq(__lookInputNormalized).Approx(0f))
        {
            LookInputVector = __lookInputNormalized;    
        }

        // Jumping input
        if (getJumpInput.Invoke())
        {
            TimeSinceJumpRequested = 0f;
            JumpRequested = true;
        }
        
    }
    public void BeforeCharacterUpdate(F32 deltaTime)
    {
        // This is called before the motor does anything
        //Debug.Log("BeforeCharacterUpdate");
        CurrentState.UpdateStates();
    }
    
    /// <summary> Called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling </summary>
    public void PostGroundingUpdate(F32 deltaTime)
    {
        //Debug.Log("PostGroundingUpdate");
        //TODO: Call UpdateStates() here instead of in Update()!!!
        //CurrentState.UpdateStates();
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its velocity should be right now. 
    /// This is the ONLY place where you can set the character's velocity
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
    {
        JumpedThisPhysicsFrame = false;
        TimeSinceJumpRequested += deltaTime;
        
        //Debug.Log("UpdateVelocity");
        //currentState.UpdateStates(ref currentVelocity, deltaTime);
        CurrentState.UpdateVelocities(ref currentVelocity, deltaTime);
    }
    
    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its rotation should be right now. 
    /// This is the ONLY place where you should set the character's rotation
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime)
    {
        //if(CantMove) return;
        //Debug.Log("UpdateRotation");
        
        CurrentState.UpdateRotations(ref currentRotation, deltaTime);
        
        //Gravity orientation
        // F32x3   __currentUp            = mul(currentRotation, up());
        // F32x3   __normalizedNegGravity = -normalizesafe(gravity);
        // F32     __orientationSpeed     = 1 - exp(-bonusOrientationSharpness * deltaTime);
        // Vector3 __smoothedGravityDir   = slerpsafe(__currentUp, __normalizedNegGravity, t: __orientationSpeed);
        //
        // currentRotation = Quaternion.FromToRotation(fromDirection: __currentUp, toDirection: __smoothedGravityDir) * currentRotation;
    }
    
    public void AfterCharacterUpdate(F32 deltaTime)
    {
        // // Handle jumping pre-ground grace period
        if (JumpRequested && TimeSinceJumpRequested > JumpPreGroundingGraceTime)
        {
            JumpRequested = false;
        }
        
        if (Motor.GroundingStatus.FoundAnyGround)
        {
            // If we're on a ground surface, reset jumping values
            if (!JumpedThisPhysicsFrame)
            {
                JumpConsumed = false;
            }
        
            TimeSinceLastAbleToJump = 0f;
        }
        else
        {
            // Keep track of time since we were last able to jump (for grace period)
            TimeSinceLastAbleToJump += deltaTime;
        }
    }

    public Bool IsColliderValidForCollisions(Collider coll)
    {
        // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        //TODO: [Garon, Walter] Add Coyote Time in here!! 
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void ProcessHitStabilityReport
    (
        Collider               hitCollider,
        Vector3                hitNormal,
        Vector3                hitPoint,
        Vector3                atCharacterPosition,
        Quaternion             atCharacterRotation,
        ref HitStabilityReport hitStabilityReport
    ) { }
    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
    }
}
