using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;        //new input

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController characterController;
    
    [SerializeField] Animator animator; //i guess there is no need in animator
    [SerializeField] float characterSpeed;

    PlayerInput playerInput;
    CharacterInput characterInput;

    private void Awake()  //stuff from codeMonkey
    {
        
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        characterInput = new CharacterInput();
        characterInput.Charactermap.Enable();
    }

    private void Update()
    {
        Vector2 inputVector = characterInput.Charactermap.Movement.ReadValue<Vector2>();
        Debug.Log(inputVector);
        MoveCharacter(inputVector);
    }

    public void MoveCharacter(Vector2 inputVector)
    {
        //i did it by CaracterController cause it will be easier to add animations later and to control physics (u should probably know that usual physics in unity sucks
        Vector3 inputDirection = new Vector3(inputVector.x, 0, inputVector.y);
        
        if (inputDirection!=Vector3.zero)
        {
            characterController.Move(inputDirection * Time.deltaTime * characterSpeed);
            transform.rotation = Quaternion.LookRotation(inputDirection);
            animator.SetTrigger("isRunningForwardTriggered");
            float animationBlendTreeValue = inputDirection.magnitude;
            animator.SetFloat("speedPercent", animationBlendTreeValue);
        }
        if (inputDirection == Vector3.zero)
        {
            animator.ResetTrigger("isRunningForwardTriggered");
            animator.SetTrigger("isIdleTriggered");
        }
    }
}
