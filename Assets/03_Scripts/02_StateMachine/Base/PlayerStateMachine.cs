using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    //animator hashes
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;

    //input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;
    bool isMovementPressed;
    bool isRunPressed;

    //const
    const float rotationFactorPerFrame = 15.0f;
    const int zero = 0;
    float gravity = -9.8f;
    float groundedGravity = -.05f;

    //moving
    public float movementSpeed = 1.0f;
    public float runMultiplier = 8.0f;

    //jumping
    bool isJumpPressed = false;
    bool isJumping = false;
    bool requireNewJumpPress = false;
    float initialJumpVelocity;
    public float maxJumpHeight = 4.0f;
    float maxJumpTime = 0.75f;

    // state variables
    PlayerBaseState currentState;
    PlayerStateFactory states;

    // getters and setters
    public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }
    public Animator Animator { get { return animator; } }
    public CharacterController CharacterController { get { return characterController; } }
    public int IsJumpingHash { get { return isJumpingHash; } }
    public int IsWalkingHash { get { return isWalkingHash; } }
    public int IsRunningHash { get { return isRunningHash; } }
    public bool RequireNewJumpPress {get { return requireNewJumpPress; } set {requireNewJumpPress = value; } }
    public bool IsJumping { set {isJumping = value; } }
    public bool IsJumpPressed {  get { return isJumpPressed; } }
    public bool IsMovementPressed { get { return isMovementPressed; } }
    public bool IsRunPressed { get { return isRunPressed; } }
    public float MovementSpeed { get { return movementSpeed; } }
    public float RunMultiplier { get { return runMultiplier; } }
    public float InitialJumpVelocity { get { return initialJumpVelocity; } }
    public float GroundedGravity { get { return groundedGravity; } }
    public float Gravity { get { return gravity; } }
    public float CurrentMovementY { get { return currentMovement.y; } set { currentMovement.y = value; } }
    public float AppliedMovementY { get { return appliedMovement.y; } set { appliedMovement.y = value; } }
    public float AppliedMovementX { get { return appliedMovement.x; } set { appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return appliedMovement.z; } set { appliedMovement.z = value; } }
    public Vector2 CurrentMovementInput { get { return currentMovementInput; } }


    private void Awake()
    {
        //set reference variables
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // setup state
        states = new PlayerStateFactory(this);
        currentState = states.Grounded();
        currentState.EnterState();

        //set hash references
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");

        // set player input callbacks
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;
        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        currentState.UpdateStates();
        characterController.Move(appliedMovement * Time.deltaTime);
    }
    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = zero;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * MovementSpeed;
        currentMovement.z = currentMovementInput.y * MovementSpeed;
        currentRunMovement.x = currentMovementInput.x * RunMultiplier;
        currentRunMovement.z = currentMovementInput.y * RunMultiplier;
        isMovementPressed = currentMovementInput.x != zero || currentMovementInput.y != zero;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        requireNewJumpPress = false;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();

    }
}
