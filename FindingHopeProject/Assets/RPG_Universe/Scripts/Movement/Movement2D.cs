using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public Rigidbody2D rb;
    public ContactFilter2D movementFilter;
    public LayerMask interactableLayer;
    public GameObject popUp;

    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private Vector2 moveDirection;

    private AnimationManager animManager;

    private void Awake()
    {
        animManager = GetComponent<AnimationManager>();
    }

    public void HandleUpdate()
    {
        ProcessInput();
        Collider2D collider = CheckInteractionSpace();
        
        if (collider)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                collider.GetComponent<Interactable>()?.Interact();
            }
            popUp.SetActive(true);
        } else
        {
            popUp.SetActive(false);
        }

        if (moveDirection != Vector2.zero)
        {
            animManager.UpdateAnimation(moveDirection);
            animManager.IsMoving(true);
            if (!TryMove(moveDirection))
            {
                if (!TryMove(new Vector2(moveDirection.x, 0)))
                {
                    TryMove(new Vector2(0, moveDirection.y));
                }
            } 
        } else
        {
            animManager.IsMoving(false);
        }
    }

    private Collider2D CheckInteractionSpace()
    {
        Vector3 facingDirection = animManager.AnimationDirection();
        Vector3 interactPosition = (transform.position + facingDirection);

        //Debug.DrawLine(transform.position, interactPosition, Color.blue);

        return Physics2D.OverlapCircle(interactPosition, 0.2f, interactableLayer);
    }

    private bool TryMove(Vector2 direction)
    {
        int count = rb.Cast(direction, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        }
        return false;
    }

    void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0) moveY = 0;
        if (moveY != 0) moveX = 0;

        moveDirection = new Vector2(moveX, moveY).normalized;
    }
}
