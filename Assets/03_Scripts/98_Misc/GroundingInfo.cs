using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    using System;

    using KinematicCharacterController;

    using Sirenix.OdinInspector;

    public sealed class GroundingInfo : MonoBehaviour
    {
        [SerializeField] private KinematicCharacterMotor motor;

        [Sirenix.OdinInspector.ReadOnly]
        [ShowInInspector]
        private CharacterGroundingReport groundingReport => motor.GroundingStatus;

        [Sirenix.OdinInspector.ReadOnly]
        [ShowInInspector]
        private bool isOnStableGround => motor.GroundingStatus.IsStableOnGround;
        
        private void Reset()
        {
            motor = GetComponent<KinematicCharacterMotor>();
        }
        
        
    }
}
