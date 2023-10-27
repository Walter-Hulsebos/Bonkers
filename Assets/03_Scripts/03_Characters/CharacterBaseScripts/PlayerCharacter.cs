namespace Bonkers.Characters
{
    using System.Linq;

    using CGTK.Utils.UnityFunc;

    using UnityEngine;
    using static UnityEngine.Mathf;
    
    using Unity.Netcode;
    using static Unity.Mathematics.math;
    using static ProjectDawn.Mathematics.math2;

    using KinematicCharacterController;

    using Sirenix.OdinInspector;

    using F32   = System.Single;
    using F32x2 = Unity.Mathematics.float2;
    using F32x3 = Unity.Mathematics.float3;
    
    using I32   = System.Int32;
    using I32x2 = Unity.Mathematics.int2;
    using I32x3 = Unity.Mathematics.int3;
    
    using Bool  = System.Boolean;
    using Rotor = Unity.Mathematics.quaternion;

    public enum CharacterState
    {
        Default,
    }

    public enum OrientationMethod
    {
        TowardsCamera,
        TowardsMovement,
    }

    public struct PlayerCharacterInputs
    {
        public F32   moveAxisForward;
        public F32   moveAxisRight;
        public Rotor cameraRotation;
        public Bool  jumpDown;
        public Bool  crouchDown;
        public Bool  crouchUp;
    }

    public struct AICharacterInputs
    {
        public F32x3 moveVector;
        public F32x3 lookVector;
    }

    public enum BonusOrientationMethod
    {
        None,
        TowardsGravity,
        TowardsGroundSlopeAndGravity,
    }

    public sealed class PlayerCharacter : NetworkBehaviour, 
                                          ICharacterController
    {
        #region Fields

        [SerializeField] private KinematicCharacterMotor motor;
        [SerializeField] private new Camera              camera;

        #if ODIN_INSPECTOR
        [BoxGroup("Stable Movement")]
        #endif
        [SerializeField] private F32 maxStableMoveSpeed = 10f;

        [SerializeField] private F32               stableMovementSharpness = 15f;
        [SerializeField] private F32               orientationSharpness    = 20f;
        [SerializeField] private OrientationMethod orientationMethod       = OrientationMethod.TowardsMovement;

        [Header(header: "Air Movement")]
        [SerializeField] private F32 maxAirMoveSpeed = 10f;

        [SerializeField] private F32 airAccelerationSpeed = 15f;
        [SerializeField] private F32 drag                 = 0.1f;

        [Header(header: "Jumping")]
        [SerializeField] private Bool allowJumpingWhenSliding = false;

        [SerializeField] private F32 jumpUpSpeed                = 10f;
        [SerializeField] private F32 jumpScalableForwardSpeed   = 0f;
        [SerializeField] private F32 jumpPreGroundingGraceTime  = 0.1f;
        [SerializeField] private F32 jumpPostGroundingGraceTime = 0.1f;

        [Header(header: "Misc")]
        [SerializeField] private Collider[] ignoredColliders;

        [SerializeField] private BonusOrientationMethod bonusOrientationMethod    = BonusOrientationMethod.TowardsGravity;
        [SerializeField] private F32                    bonusOrientationSharpness = 20f;
        [SerializeField] private F32x3                  gravity                   = new (x: 0, y: -30f, z: 0);
        //[SerializeField] private Transform              meshRoot;
        
        [SerializeField] private UnityFunc<F32x2> getMoveInputVector;
        [SerializeField] private UnityFunc<Bool>  getJumpInput;

        [field:KinematicCharacterController.ReadOnly]
        [field:SerializeField]
        public CharacterState CurrentCharacterState { get; private set; }

        private readonly Collider[]   _probedColliders = new Collider[8];
        private          RaycastHit[] _probedHits      = new RaycastHit[8];
        private          F32x3        _moveInputVector;
        private          F32x3        _lookInputVector;
        private          Bool         _jumpRequested           = false;
        private          Bool         _jumpConsumed            = false;
        private          Bool         _jumpedThisFrame         = false;
        private          F32          _timeSinceJumpRequested  = Infinity;
        private          F32          _timeSinceLastAbleToJump = 0f;
        private          F32x3        _internalVelocityAdd     = F32x3.zero;

        private F32x3 _lastInnerNormal = F32x3.zero;
        private F32x3 _lastOuterNormal = F32x3.zero;
        
        //private Bool CantMove => !IsOwner || !Application.isFocused;

        #endregion

        #region Methods
        
        private void Reset()
        {
            motor = GetComponent<KinematicCharacterMotor>();

            ignoredColliders = new []{ transform.GetComponent<Collider>(), };

            //meshRoot = transform.Find("Meshes");
        }

        private void Awake()
        {
            // Handle initial state
            TransitionToState(newState: CharacterState.Default);

            // Assign the characterController to the motor
            motor.CharacterController = this;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
        }

        /// <summary>
        /// Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToState(CharacterState newState)
        {
            CharacterState __tmpInitialState = CurrentCharacterState;
            OnStateExit(state: __tmpInitialState, toState: newState);
            CurrentCharacterState = newState;
            OnStateEnter(state: newState, fromState: __tmpInitialState);
        }

        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default: { break; }
            }
        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default: { break; }
            }
        }

        private void Update()
        {
            //if(!IsOwner || !Application.isFocused) return;
            if(!Application.isFocused) return;
            
            F32x2 __moveAxis = getMoveInputVector.Invoke();

            F32x3 __moveInputVector = Vector3.ClampMagnitude(vector: new F32x3(x: __moveAxis.x, y: 0f, z: __moveAxis.y), maxLength: 1f);

            Rotor __cameraRotation = camera.transform.rotation;
            
            F32x3 __cameraPlanarDirection = normalize(Vector3.ProjectOnPlane(vector: mul(__cameraRotation, forward()), planeNormal: motor.CharacterUp));
            
            if (lengthsq(__cameraPlanarDirection) == 0f)
            {
                __cameraPlanarDirection = normalize(Vector3.ProjectOnPlane(vector: mul(__cameraRotation, up()), planeNormal: motor.CharacterUp));
            }
            
            Rotor __cameraPlanarRotation = Rotor.LookRotation(forward: __cameraPlanarDirection, up: motor.CharacterUp);

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    // Move and look inputs
                    _moveInputVector = mul(__cameraPlanarRotation, __moveInputVector);

                    _lookInputVector = orientationMethod switch
                    {
                        OrientationMethod.TowardsCamera   => __cameraPlanarDirection,
                        OrientationMethod.TowardsMovement => normalize(_moveInputVector),
                        _                                 => _lookInputVector,
                    };

                    // // Jumping input
                    if (getJumpInput.Invoke())
                    {
                        _timeSinceJumpRequested = 0f;
                        _jumpRequested          = true;
                    }

                    break;
                }
            }
        }

        private Quaternion _tmpTransientRot;

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(F32 deltaTime) { }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now. 
        /// This is the ONLY place where you should set the character's rotation
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime)
        {
            if(!Application.isFocused) return;
            
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    //if(CantMove) return;
                    
                    if (lengthsq(_lookInputVector) > 0f && orientationSharpness > 0f)
                    {
                        // Smoothly interpolate from current to target look direction
                        //Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Exp(power: -orientationSharpness * deltaTime));
                        Vector3 __smoothedLookInputDirection = normalize(slerp(motor.CharacterForward, _lookInputVector, t: 1 - exp(-orientationSharpness * deltaTime)));

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(forward: __smoothedLookInputDirection, upwards: motor.CharacterUp);
                    }

                    F32x3 __currentUp = mul(currentRotation, up());

                    switch (bonusOrientationMethod)
                    {
                        case BonusOrientationMethod.TowardsGravity:
                        {
                            // Rotate from current up to invert gravity
                            F32x3   __normalizedNegGravity = -normalizesafe(gravity);
                            F32     __orientationSpeed     = 1 - exp(-bonusOrientationSharpness * deltaTime);
                            Vector3 __smoothedGravityDir   = slerpsafe(__currentUp, __normalizedNegGravity, t: __orientationSpeed);

                            currentRotation = Quaternion.FromToRotation(fromDirection: __currentUp, toDirection: __smoothedGravityDir) * currentRotation;
                            break;
                        }

                        case BonusOrientationMethod.TowardsGroundSlopeAndGravity when motor.GroundingStatus.IsStableOnGround:
                        {
                            F32x3 __initialCharacterBottomHemiCenter = (F32x3)motor.TransientPosition + __currentUp * motor.Capsule.radius;

                            Vector3 __smoothedGroundNormal = slerp(motor.CharacterUp, motor.GroundingStatus.GroundNormal, t: 1 - exp(-bonusOrientationSharpness * deltaTime));

                            currentRotation = Quaternion.FromToRotation(fromDirection: __currentUp, toDirection: __smoothedGroundNormal) * currentRotation;

                            // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                            motor.SetTransientPosition(newPos: __initialCharacterBottomHemiCenter + mul(currentRotation, down()) * motor.Capsule.radius);
                            break;
                        }

                        case BonusOrientationMethod.TowardsGroundSlopeAndGravity:
                        {
                            Vector3 __smoothedGravityDir = slerp(__currentUp, normalize(-gravity), t: 1 - exp(-bonusOrientationSharpness * deltaTime));

                            currentRotation = Quaternion.FromToRotation(fromDirection: __currentUp, toDirection: __smoothedGravityDir) * currentRotation;
                            break;
                        }

                        default:
                        {
                            Vector3 __smoothedGravityDir = slerp( __currentUp, up(), t: 1 - exp(-bonusOrientationSharpness * deltaTime));
                            
                            currentRotation = Quaternion.FromToRotation(fromDirection: __currentUp, toDirection: __smoothedGravityDir) * currentRotation;
                            break;
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now. 
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
        {
            if(!Application.isFocused) return;
            
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    //if(CantMove) return;
                    
                    // Ground movement
                    if (motor.GroundingStatus.IsStableOnGround)
                    {
                        F32 __currentVelocityMagnitude = currentVelocity.magnitude;

                        F32x3 __effectiveGroundNormal = motor.GroundingStatus.GroundNormal;

                        // Reorient velocity on slope
                        currentVelocity = motor.GetDirectionTangentToSurface(direction: currentVelocity, surfaceNormal: __effectiveGroundNormal) * __currentVelocityMagnitude;

                        // Calculate target velocity
                        F32x3 __inputRight = cross(_moveInputVector, motor.CharacterUp);

                        F32x3 __reorientedInput = normalize(cross(__effectiveGroundNormal, __inputRight)) * length(_moveInputVector);

                        F32x3 __targetMovementVelocity = __reorientedInput * maxStableMoveSpeed;

                        // Smooth movement Velocity
                        currentVelocity = lerp(currentVelocity, __targetMovementVelocity, t: 1f - exp(-stableMovementSharpness * deltaTime));
                    }
                    // Air movement
                    else
                    {
                        // Add move input
                        if (lengthsq(_moveInputVector) > 0f)
                        {
                            F32x3 __addedVelocity = _moveInputVector * airAccelerationSpeed * deltaTime;
                            
                            F32x3 __currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(vector: currentVelocity, planeNormal: motor.CharacterUp);

                            // Limit air velocity from inputs
                            if (length(__currentVelocityOnInputsPlane) < maxAirMoveSpeed)
                            {
                                // clamp addedVel to make total vel not exceed max vel on inputs plane
                                F32x3 __newTotal = Vector3.ClampMagnitude(vector: __currentVelocityOnInputsPlane + __addedVelocity, maxLength: maxAirMoveSpeed);
                                __addedVelocity = __newTotal - __currentVelocityOnInputsPlane;
                            }
                            else
                            {
                                // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                                if (dot(__currentVelocityOnInputsPlane, __addedVelocity) > 0f)
                                {
                                    __addedVelocity = Vector3.ProjectOnPlane(vector: __addedVelocity, planeNormal: normalize(__currentVelocityOnInputsPlane));
                                }
                            }

                            // Prevent air-climbing sloped walls
                            if (motor.GroundingStatus.FoundAnyGround)
                            {
                                if (dot((F32x3)currentVelocity + __addedVelocity, __addedVelocity) > 0f)
                                {
                                    F32x3 __perpenticularObstructionNormal = normalize(cross
                                                                                (
                                                                                           cross
                                                                                           (
                                                                                               motor.CharacterUp,
                                                                                               motor.GroundingStatus.GroundNormal
                                                                                           ),
                                                                                           motor.CharacterUp
                                                                                       ));

                                    __addedVelocity = Vector3.ProjectOnPlane(vector: __addedVelocity, planeNormal: __perpenticularObstructionNormal);
                                }
                            }

                            // Apply added velocity
                            currentVelocity += (Vector3)__addedVelocity;
                        }

                        // Gravity
                        currentVelocity += (Vector3)(gravity * deltaTime);

                        // Drag
                        currentVelocity *= 1f / (1f + drag * deltaTime);
                    }

                    // Handle jumping
                    _jumpedThisFrame        =  false;
                    _timeSinceJumpRequested += deltaTime;

                    if (_jumpRequested)
                    {
                        // See if we actually are allowed to jump
                        if (!_jumpConsumed &&
                            ((allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround) ||
                             _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime))
                        {
                            // Calculate jump direction before ungrounding
                            F32x3 __jumpDirection = motor.CharacterUp;

                            if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
                            {
                                __jumpDirection = motor.GroundingStatus.GroundNormal;
                            }

                            // Makes the character skip ground probing/snapping on its next update. 
                            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                            motor.ForceUnground();

                            // Add to the return velocity and reset jump state
                            currentVelocity  += (Vector3)(__jumpDirection * jumpUpSpeed - (F32x3)Vector3.Project(vector: currentVelocity, onNormal: motor.CharacterUp));
                            currentVelocity  += (Vector3)(_moveInputVector * jumpScalableForwardSpeed);
                            _jumpRequested   =  false;
                            _jumpConsumed    =  true;
                            _jumpedThisFrame =  true;
                        }
                    }

                    // Take into account additive velocity
                    if (lengthsq(_internalVelocityAdd) > 0f)
                    {
                        currentVelocity      += (Vector3)_internalVelocityAdd;
                        _internalVelocityAdd =  F32x3.zero;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(F32 deltaTime)
        {
            if(!Application.isFocused) return;
            
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    // Handle jump-related values
                    {
                        // Handle jumping pre-ground grace period
                        if (_jumpRequested && _timeSinceJumpRequested > jumpPreGroundingGraceTime) { _jumpRequested = false; }

                        if (allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround)
                        {
                            // If we're on a ground surface, reset jumping values
                            if (!_jumpedThisFrame) { _jumpConsumed = false; }

                            _timeSinceLastAbleToJump = 0f;
                        }
                        else
                        {
                            // Keep track of time since we were last able to jump (for grace period)
                            _timeSinceLastAbleToJump += deltaTime;
                        }
                    }

                    break;
                }
            }
        }

        public void PostGroundingUpdate(F32 deltaTime)
        {
            // Handle landing and leaving ground
            // if (motor.GroundingStatus.IsStableOnGround       && !motor.LastGroundingStatus.IsStableOnGround) { OnLanded(); }
            // else if (!motor.GroundingStatus.IsStableOnGround && motor.LastGroundingStatus.IsStableOnGround) { OnLeaveStableGround(); }
        }

        public Bool IsColliderValidForCollisions(Collider coll)
        {
            if (ignoredColliders.Length == 0) { return true; }

            if (ignoredColliders.Contains(coll)) { return false; }

            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

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

        //protected void OnLanded() { }

        //protected void OnLeaveStableGround() { }

        public void OnDiscreteCollisionDetected(Collider hitCollider) { }
        
        #endregion
    }
}