using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour, IBoxParentObject
{
    private const float GRAVITY = -9.81f;

    public static Player Instance { get; private set; }

    [Header("Components")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private CharacterController characterController;

    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 8f;
    [SerializeField] private float jumpForce = 1.3f;
    [SerializeField] private float gravityMultiplier = 0.01f;

    [Header("Interaction Settings")]
    [SerializeField] private LayerMask interactionLayer;

    [Header("Grab Settings")]
    [SerializeField] private Transform grabPosition;
    [SerializeField] private float grabbingSpeed = 3f;
    [SerializeField] private float turningSpeedWhileGrabbing = 4f;

    private Box grabbedBox;
    private bool hasBox = false;

    private Transform mainCameraTransform;
    private float verticalVelocity;
    private float movementSpeed;
    private bool isRunning;
    private bool isWalking;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("There is more than one player instance!");
        
        Instance = this;
        movementSpeed = walkingSpeed;
    }

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;

        gameInput.OnPlayerInteractAction += GameInput_OnPlayerInteractAction;
        gameInput.OnPlayerJumpAction += GameInput_OnPlayerJumpAction;
        gameInput.OnPlayerRunAction += GameInput_OnPlayerRunAction;
    }

    private void GameInput_OnPlayerInteractAction(object sender, EventArgs e)
    {
        if (hasBox)
        {
            DropBox();
            return;
        }

        float interactRange = 2f;

        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange, interactionLayer);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(this);
                break;
            }
        }
    }

    private void GameInput_OnPlayerRunAction(object sender, EventArgs e)
    {
        if (isRunning)
        {
            movementSpeed = walkingSpeed;
            isRunning = false;
        }
        else
        {
            movementSpeed = runningSpeed;
            isRunning = true;
        }
    }

    private void GameInput_OnPlayerJumpAction(object sender, System.EventArgs e)
    {
        if (!IsGrounded())
            return;

        verticalVelocity += jumpForce;
    }

    private void Update()
    {
        if (!hasBox)
            HandleMovement();
        else
            HandleBoxMovement();
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = inputVector.y * mainCameraTransform.forward;
        moveDir += inputVector.x * mainCameraTransform.right;
        moveDir.y = 0;
        moveDir = moveDir.normalized * movementSpeed * Time.deltaTime;

        if (characterController.isGrounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = 0.0f;
        }
        else
        {
            verticalVelocity += GRAVITY * gravityMultiplier * Time.deltaTime;
            moveDir.y = verticalVelocity;
        }

        characterController.Move(moveDir);

        isWalking = moveDir.x != 0.0f || moveDir.z != 0.0f;
        
        float rotateSpeed = 10f;
        moveDir.y = 0;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void HandleBoxMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = inputVector.y * mainCameraTransform.forward;
        moveDir += inputVector.x * mainCameraTransform.right;
        moveDir.y = 0;
        moveDir = moveDir.normalized * grabbingSpeed * Time.deltaTime;

        if (characterController.isGrounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = 0.0f;
        }
        else
        {
            verticalVelocity += GRAVITY * gravityMultiplier * Time.deltaTime;
            moveDir.y = verticalVelocity;
        }

        characterController.Move(moveDir);

        moveDir.y = 0;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turningSpeedWhileGrabbing);
    }

    private bool IsGrounded()
    {   
        float maxDistance = 0.1f;

        return Physics.Raycast(transform.position, Vector3.down, maxDistance);
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public void GrabBox(Box box)
    {
        grabbedBox = box;
        hasBox = true;
    }

    public void DropBox()
    {   
        grabbedBox.RemoveParent();

        grabbedBox = null;
        hasBox = false;
    }

    public Transform GetGrabPositionTransform()
    {
        return grabPosition;
    }
}
