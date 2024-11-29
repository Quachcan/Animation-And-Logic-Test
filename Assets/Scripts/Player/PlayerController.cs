using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    private  InputManager playerInput;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    [SerializeField]
    private float speed;
    private float rotationFactorPerFrame = 1f;

    private void Awake()
    {
        playerInput = new InputManager();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInput.PlayerControls.Move.started += OnMovementInput;
        playerInput.PlayerControls.Move.canceled += OnMovementInput;
        playerInput.PlayerControls.Move.performed += OnMovementInput; 
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;

    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;
        if(isMovementPressed)
        {
        Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
        transform.rotation = Quaternion.Slerp(targetRotation, currentRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if(isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
    }

    void Update()
    {
        HandleAnimation();
        HandleRotation();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    void FixedUpdate()
    {
    }

    void OnEnable()
    {
        playerInput.PlayerControls.Enable();
    }

    void OnDisable()
    {
        playerInput.PlayerControls.Disable();
    }
}
