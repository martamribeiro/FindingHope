using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private GameInput gameInput;
    [SerializeField] private float moveSpeed = 7f;

    private bool isWalking;
    private Transform mainCameraTransform;

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
        moveDir.Normalize();

        Debug.DrawLine(transform.position, transform.position + moveDir * 5, Color.green);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Attempt only X movement
            Vector3 moveDirX = (inputVector.x * mainCameraTransform.right).normalized;
            moveDirX.y = 0;
            Debug.DrawLine(transform.position, transform.position + moveDirX * 5, Color.red);

            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Can move only on the X axis
                moveDir = moveDirX;
            }
            else
            {
                // Attempt only z movement
                Vector3 moveDirZ = (inputVector.y * mainCameraTransform.forward).normalized;
                moveDirZ.y = 0;
                Debug.DrawLine(transform.position, transform.position + moveDirX * 5, Color.blue);

                canMove = moveDir.y != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can move only on the Z axis
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero; 

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
}
