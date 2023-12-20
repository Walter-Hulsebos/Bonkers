using System;
using UnityEngine;

using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

using F32 = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Bool = System.Boolean;
using KinematicCharacterController;
using Unity.VisualScripting;

namespace Bonkers._02_StateMachine.States
{
    public class PlayerWallSlideState : PlayerBaseState
    {


        #region Variables

        private bool IsSliding;
        private float SlidingSpeed = -1.5f;
        private float RotationSpeed = 10;

        //RayCast Related
        private float MaxRayDistance = 10000;
        private Vector3 RayDirection;
        private RaycastHit Hit;

        private float ShortestDistance = 100;
        private Vector3 LookDirection;

        #endregion

        #region Constructor

        public PlayerWallSlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
        }

        #endregion

        #region Enter/Exit

        public override void EnterState()
        {
            IsSliding = true;
            Debug.Log("Starting Wall Slide");

            //Shooting Raycasts in all 9 directions
            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                        RayDirection = new Vector3(1, 0, 0);
                        break;
                    case 1:
                        RayDirection = new Vector3(0, 0, 1);
                        break;
                    case 2:
                        RayDirection = new Vector3(-1, 0, 0);
                        break;
                    case 3:
                        RayDirection = new Vector3(0, 0, -1);
                        break;
                    case 4:
                        RayDirection = new Vector3(1, 0, 1);
                        break;
                    case 5:
                        RayDirection = new Vector3(-1, 0, -1);
                        break;
                    case 6:
                        RayDirection = new Vector3(1, 0, -1);
                        break;
                    case 7:
                        RayDirection = new Vector3(-1, 0, 1);
                        break;
                }

                if (Physics.Raycast(Ctx.transform.position, RayDirection, out Hit, MaxRayDistance))
                {
                    Debug.DrawRay(Ctx.transform.position, RayDirection * MaxRayDistance, Color.red, 1);
                    if (Hit.distance < ShortestDistance)
                    {
                        ShortestDistance = Hit.distance;
                        LookDirection = RayDirection * -1;
                        Debug.DrawRay(Ctx.transform.position, RayDirection * MaxRayDistance, Color.yellow, 6);
                        Debug.DrawRay(Ctx.transform.position, LookDirection * MaxRayDistance, Color.blue, 6);
                    }
                }
            }
        }

        public override void ExitState()
        {
            IsSliding = false;
            Debug.Log("Stopped Wall Slide");
        }

        #endregion

        #region Update
        protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            currentVelocity = new Vector3(0, SlidingSpeed, 0);
           
        }

        protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) 
        {
            F32 orientSharpness = 20f;

            if (LookDirection != null)
            {
                F32x3 __forward = Ctx.Motor.CharacterForward;

                F32x3 __smoothedLookInputDirection = normalizesafe(slerp(start: __forward, end: LookDirection, t: 1 - exp(-orientSharpness * deltaTime)));
                currentRotation = Quaternion.LookRotation(forward: __smoothedLookInputDirection, upwards: Ctx.Motor.CharacterUp);
            }
        }

        #endregion

        #region Switch States

        public override void CheckSwitchStates()
        {
            if (Ctx.Motor.GroundingStatus.IsStableOnGround)
            {
                SwitchState(Factory.Grounded());
            }
        }

        #endregion
    }
}