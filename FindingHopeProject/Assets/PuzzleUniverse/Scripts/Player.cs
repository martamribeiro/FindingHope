using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private GameInput gameInput;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float gravityMultiplier = 3.0f;

    private Transform mainCameraTransform;
    private bool isWalking;
    private float gravity = -9.81f;
    private float velocity;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("There is more than one player instance!");
        
        Instance = this;
    }

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        HandleMovement();
    }
    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = inputVector.y * mainCameraTransform.forward;
        moveDir += inputVector.x * mainCameraTransform.right;
        moveDir.y = 0;
        moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;

        if (characterController.isGrounded && velocity < 0.0f)
        {
            velocity = 0.0f;
        }
        else
        {
            velocity += gravity * gravityMultiplier * Time.deltaTime;
            moveDir.y = velocity;
        }

        characterController.Move(moveDir);

        //isWalking = moveDir != Vector3.zero; 
        
        float rotateSpeed = 10f;
        moveDir.y = 0;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
}
