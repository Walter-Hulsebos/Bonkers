namespace Bonkers.Characters.Shared
{

    using System;

    using CGTK.Utils.Extensions.Math.Math;
    using CGTK.Utils.UnityFunc;

    using KinematicCharacterController;

    using Sirenix.OdinInspector;

    using UnityEngine;

    using static UnityEngine.Mathf;

    using static Unity.Mathematics.math;

    using static ProjectDawn.Mathematics.math2;

//using static Bonkers.Characters.OrientationMethod;

    using F32   = System.Single;
    using F32x2 = Unity.Mathematics.float2;
    using F32x3 = Unity.Mathematics.float3;

    [Serializable]
    public sealed class IdleState : PlayerState
    {

        #region References

        #if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(groupName: "References", expanded: true)]
        #endif
        [SerializeField] private Animator animator;

        #if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(groupName: "References", expanded: true)]
        #endif
        [SerializeField] private KinematicCharacterMotor motor;

        #if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(groupName: "References", expanded: true)]
        #endif
        [SerializeField] private CharacterInputs inputs;
        
        #if UNITY_EDITOR
        private void Reset()
        {
            animator = GetComponent<Animator>();
            motor    = GetComponent<KinematicCharacterMotor>();
            inputs   = GetComponent<CharacterInputs>();
            
            if(animator == null) { animator = GetComponentInChildren<Animator>(); }
            if(motor    == null) { motor    = GetComponentInChildren<KinematicCharacterMotor>(); }
            if(inputs   == null) { inputs   = GetComponentInChildren<CharacterInputs>(); }
            
            if(animator == null) { Debug.LogWarning(message: $"No {nameof(Animator)} found on {name} or its children.",                context: this); }
            if(motor    == null) { Debug.LogWarning(message: $"No {nameof(KinematicCharacterMotor)} found on {name} or its children.", context: this); }
            if(inputs   == null) { Debug.LogWarning(message: $"No {nameof(CharacterInputs)} found on {name} or its children.",         context: this); }
        }
        #endif

        #endregion
        
        #region Variables

        #endregion
        

        #region Enter/Exit

        public override void OnEnter()
        {
            Debug.Log(message: "Entering Idle State");
            
            
        }

        public override void OnExit()
        {
            Debug.Log(message: "Exiting Idle State");
            
            
        }

        #endregion

        #region Update

        public override void UpdateVelocity(ref Vector3 currentVelocity, F32 deltaTime)
        {
            
        }

        public override void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime)
        {
            
        }

        #endregion

    }
}