using Bonkers.Controls;
using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bonkers
{
    public class AnimatorController : MonoBehaviour
    {
        Animator animator;

        int WalkingHash;

        //no running input or running anim in kinematic controller, still available for future use
        //int RunningHash;

        int JumpHash;
        int IdleHash;

        Controls.Controls input;

        Vector2 currentMovement;
        bool movementPressed;

        //no running input or running anim in kinematic controller, still available for future use
        //bool runPressed;

        bool jumpPressed;
         
        //  !!!  set motor manually for testing, animator and controller are on different components  !!!
        public KinematicCharacterMotor Motor;

        private void Awake()
        {
            input = new Controls.Controls();

            input.Gameplay.Move.performed += ctx =>
            {
                currentMovement = ctx.ReadValue<Vector2>();
                movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
            };

            //When player presses the jump button jumpPressed is set to true
            input.Gameplay.Jump.performed += ctx => jumpPressed = ctx.ReadValueAsButton();

            //When player lets go of (w,a,s,d) animation stops playing
            input.Gameplay.Move.canceled += ctx => movementPressed = false;

            //When player lets go of the jump button it instantly sets jumpPressed to false and the animation stops playing !!!
            input.Gameplay.Jump.canceled += ctx => jumpPressed = false;
        }

        void Start()
        {
            animator = GetComponent<Animator>();

            //set the hashes to the strings of the animator parameters
            WalkingHash = Animator.StringToHash("Walking");

            //no running input or running anim in kinematic controller, still available for future use
            //RunningHash = Animator.StringToHash("Running");

            JumpHash = Animator.StringToHash("Jumping");
            IdleHash = Animator.StringToHash("Idle");
        }

        void Update()
        {
            animMovement();
            animJump();
        }

        void animMovement()
        {
            bool isWalking = animator.GetBool(WalkingHash);

            //no running input or running anim in kinematic controller, still available for future use
            //bool isRunning = animator.GetBool(RunningHash);

            if(movementPressed && !isWalking)
            {
                animator.SetBool(WalkingHash, true);
                animator.SetBool(IdleHash, false);
            }

            if(!movementPressed && isWalking)
            {
                animator.SetBool(WalkingHash, false);
                animator.SetBool(IdleHash, true);
            }
        }

        void animJump()
        {
            bool isJumping = animator.GetBool(JumpHash);
            
            //if jumping input is pressed and jumping parameter in animator is false, set jumping parameter to true
            if (jumpPressed && !isJumping)
            {
                animator.SetBool(JumpHash, true);
            }

            //if jumping parameter in animator is true and the player is grounded, jumping parameter is set to false
            if(isJumping && Motor.GroundingStatus.IsStableOnGround)
            {
                animator.SetBool(JumpHash, false);
            }
        }

        public void animSpecial1()
        {
            animator.Play("Base Layer.Special", 0, 0.25f);
        }
        public void animSpecial2()
        {
            animator.Play("Base Layer.Special", 0, 0.25f);
        }

        public void animSpecial3()
        {
            animator.Play("Base Layer.Special", 0, 0.25f);
        }

        void OnEnable()
        {
            input.Gameplay.Enable();
        }
        void OnDisable()
        {
            input.Gameplay.Disable();   
        }
    }
}
