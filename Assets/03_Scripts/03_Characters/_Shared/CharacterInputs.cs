namespace Bonkers.Characters.Shared
{
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    using UnityEngine;
    using static UnityEngine.Mathf;
    using static Unity.Mathematics.math;
    using static ProjectDawn.Mathematics.math2;
    
    using JetBrains.Annotations;
    
    using CGTK.Utils.Extensions.Math.Math;
    using CGTK.Utils.UnityFunc;

    using KinematicCharacterController;

    using F32   = System.Single;
    using F32x2 = Unity.Mathematics.float2;
    using F32x3 = Unity.Mathematics.float3;
    
    using I32   = System.Int32;
    using I32x2 = Unity.Mathematics.int2;
    using I32x3 = Unity.Mathematics.int3;
    
    using Bool  = System.Boolean;
    using Rotor = Unity.Mathematics.quaternion;

    public sealed class CharacterInputs : MonoBehaviour
    {

        #region References

        #if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(groupName: "References", expanded: true)]
        #endif
        [SerializeField] private KinematicCharacterMotor motor;
        
        #if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(groupName: "References", expanded: true)]
        #endif
        [SerializeField] public Camera camera;
        
        #if UNITY_EDITOR
        private void Reset()
        {
            motor  = GetComponent<KinematicCharacterMotor>();
            camera = GetComponent<Camera>();
            
            if(motor  == null) { motor  = GetComponentInChildren<KinematicCharacterMotor>(); }
            if(camera == null) { camera = GetComponentInChildren<Camera>(); }
            
            if(motor  == null) { Debug.LogWarning(message: $"No {nameof(KinematicCharacterMotor)} found on {name} or its children.", context: this); }
            if(camera == null) { Debug.LogWarning(message: $"No {nameof(Camera)} found on {name} or its children.",                  context: this); }
        }
        #endif

        #endregion
        
        #region Inputs
        
        [SerializeField] private UnityFunc<F32x2> getMoveInput;
        [SerializeField] private UnityFunc<Bool>  getJumpInput;
        [SerializeField] private UnityFunc<Bool>  getSpecial1Input;
        
        [PropertySpace]
        
        [field:SerializeField] public F32 JumpPreGroundingGraceTime  { get; private set; } = 0.1f;
        [field:SerializeField] public F32 JumpPostGroundingGraceTime { get; private set; } = 0.1f;
        
        public F32x3 CameraPlanarDir { get; private set; } = F32x3.zero;
        public Rotor CameraPlanarRot { get; private set; } = Rotor.identity;

        
        public F32x3 MoveInputVector   { get; private set; } = F32x3.zero;
        public F32x3 LookInputVector   { get; private set; } = F32x3.zero;
        public Bool  Special1Requested => getSpecial1Input.Invoke();

        public Bool JumpRequested           { get; internal set; } = false;
        public Bool JumpConsumed            { get; internal set; } = false;
        public Bool JumpedThisPhysicsFrame  { get; internal set; } = false;
        public F32  TimeSinceJumpRequested  { get; internal set; } = Infinity;
        public F32  TimeSinceLastAbleToJump { get; internal set; } = 0f;

        #endregion

        #region Methods

        private void Update()
        {
            //if(!Application.isFocused) return;
            //JumpedThisPhysicsFrame =  false;
            TimeSinceJumpRequested += Time.deltaTime;
            
            
            F32x2 __moveAxis = getMoveInput.Invoke();

            F32x3 __moveInputVector = Vector3.ClampMagnitude(vector: new F32x3(x: __moveAxis.x, y: 0f, z: __moveAxis.y), maxLength: 1f);

            Rotor __cameraRotation = camera.transform.rotation;
            CameraPlanarDir = normalizesafe(Vector3.ProjectOnPlane(vector: mul(__cameraRotation, forward()), planeNormal: motor.CharacterUp));
            if (lengthsq(CameraPlanarDir) == 0f)
            {
                CameraPlanarDir = normalizesafe(Vector3.ProjectOnPlane(vector: mul(__cameraRotation, up()), planeNormal: motor.CharacterUp));
            }
            CameraPlanarRot = Rotor.LookRotation(forward: CameraPlanarDir, up: motor.CharacterUp);

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
                JumpRequested          = true;
            }

            //Attack Input
            // if(getSpecial1Input.Invoke())
            // {
            //    Special1Requested = true;
            // }
            //     
        
        }

        #endregion
        
    }
}