using System;
using UnityEngine;

using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

using F32 = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Bool = System.Boolean;


    public sealed class PlayerWallJumpState : PlayerBaseState
    {

    public bool _canWallJump;
        public PlayerWallJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){}

        protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }

