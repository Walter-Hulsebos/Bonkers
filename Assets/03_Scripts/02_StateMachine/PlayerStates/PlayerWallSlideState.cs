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

using UnityEngine.InputSystem;
using Bonkers.Controls;
using CGTK.Utils.Extensions.Math.Math;

namespace Bonkers._02_StateMachine.States
{
    public class PlayerWallSlideState : PlayerBaseState
    {


        #region Variables

        private bool IsSliding;
        private float SlidingSpeed = -2.5f;
        private float RotationSpeed = 10;

        //RayCast Related
        private float MaxRayDistance = 10000;
        private Vector3 RayDirection;
        private RaycastHit Hit;

        private float ShortestDistance = 100;
        private Vector3 LookDirection;

        //Walking out of wall slide requirements
        private Vector3 OriginalPointOfImpact;
        private bool CheckDistance;
        private float minDistanceToOrigin = 2.5f;
        private bool SwitchToAir;

        [SerializeField] private F32 maxSpeedForInputs = 10f;
        [SerializeField] private F32 accSpeed = 15f;
        [SerializeField] private F32 drag = 0.1f;
        [SerializeField] private F32 orientSharpness = 20f;

        //Fabian Wall Jump Requirements
        [SerializeField] private bool canWallJump = false;
        public Controls.Controls playerControls;
        public InputAction wallJumpAction;
        public InputAction moveHorizontal;
        public InputAction moveVertical;

        #endregion

        #region Constructor

        public PlayerWallSlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            IsRootState = true;

            playerControls = new Controls.Controls();

            wallJumpAction = playerControls.Gameplay.Jump;
            moveHorizontal = playerControls.Gameplay.Move_Hor;
            moveVertical = playerControls.Gameplay.Move_Ver;
        }

        #endregion

        #region Enter/Exit

        public override void EnterState()
        {
            wallJumpAction.Enable();
            moveHorizontal.Enable();
            moveVertical.Enable();
            Ctx.Anims.SetTrigger(Ctx.WallSlideHash);


            IsSliding = true;
            canWallJump = true;
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

            OriginalPointOfImpact = Ctx.transform.position;
        }

        public override void ExitState()
        {
            wallJumpAction.Disable();
            moveHorizontal.Disable();
            moveVertical.Disable();

            IsSliding = false;
            Debug.Log("Stopped Wall Slide");
        }

        #endregion

        #region Update
        protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (CheckDistance == false)
            {
                currentVelocity = new Vector3(0, SlidingSpeed, 0);
            }

            else if(CheckDistance == true)
            {
                Ctx.Anims.SetTrigger(Ctx.JumpHash);

                F32x3 __moveInputVector = Ctx.MoveInputVector;

                if (lengthsq(__moveInputVector).Approx(0f)) return;

                F32x3 __addedVelocity = __moveInputVector * accSpeed * deltaTime;

                F32x3 __currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(vector: currentVelocity, planeNormal: Ctx.Motor.CharacterUp);

                // Limit air velocity from inputs
                if (length(__currentVelocityOnInputsPlane) < maxSpeedForInputs)
                {
                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                    F32x3 __newTotal = Vector3.ClampMagnitude(vector: __currentVelocityOnInputsPlane + __addedVelocity, maxLength: maxSpeedForInputs);
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
                if (Ctx.Motor.GroundingStatus.FoundAnyGround)
                {
                    if (dot((F32x3)currentVelocity + __addedVelocity, __addedVelocity) > 0f)
                    {
                        F32x3 __perpenticularObstructionNormal = normalize
                            (cross(cross(Ctx.Motor.CharacterUp, Ctx.Motor.GroundingStatus.GroundNormal), Ctx.Motor.CharacterUp));

                        __addedVelocity = Vector3.ProjectOnPlane(vector: __addedVelocity, planeNormal: __perpenticularObstructionNormal);
                    }
                }

                // Apply drag
                __addedVelocity *= 1f / (1f + (drag * deltaTime));

                // Apply added velocity
                currentVelocity += (Vector3)__addedVelocity;

                Debug.Log(Vector3.Distance(OriginalPointOfImpact, Ctx.transform.position) + " vs " + minDistanceToOrigin);
                if (Vector3.Distance(OriginalPointOfImpact, Ctx.transform.position) > minDistanceToOrigin)
                {
                    SwitchToAir = true;
                }
            }            
        }

        protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) 
        {
            if (CheckDistance == false)
            {
                F32 orientSharpness = 20f;

                if (LookDirection != null)
                {
                    F32x3 __forward = Ctx.Motor.CharacterForward;

                    F32x3 __smoothedLookInputDirection = normalizesafe(slerp(start: __forward, end: LookDirection, t: 1 - exp(-orientSharpness * deltaTime)));
                    currentRotation = Quaternion.LookRotation(forward: __smoothedLookInputDirection, upwards: Ctx.Motor.CharacterUp);
                }
            }
            else
            {
                if (lengthsq(Ctx.LookInputVector).Approx(0f)) return;

                // Smoothly interpolate from current to target look direction
                //Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Exp(power: -orientationSharpness * deltaTime));
                F32x3 __forward = Ctx.Motor.CharacterForward;
                F32x3 __look = Ctx.LookInputVector;

                F32x3 __smoothedLookInputDirection = normalizesafe(slerp(start: __forward, end: __look, t: 1 - exp(-orientSharpness * deltaTime)));

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
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

            moveHorizontal.started += Button =>
            {
                if (CheckDistance == false)
                {
                    CheckDistance = true;
                }
            };

            moveVertical.started += Button =>
            {
                if (CheckDistance == false)
                {
                    CheckDistance = true;
                }
            };

            if (SwitchToAir)
            {
                CheckDistance = false;
                SwitchState(Factory.Air());
            }

            wallJumpAction.started += Button =>
            {
                if(canWallJump == true)
                {
                    canWallJump = false;
                    SwitchState(Factory.WallJump());
                }
            };           

        }

        #endregion
    }
}