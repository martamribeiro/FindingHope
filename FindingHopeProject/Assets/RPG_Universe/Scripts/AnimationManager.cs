using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateAnimation(Vector2 stateDirection)
    {
        if (stateDirection.x < 0) spriteRenderer.flipX = true;
        else spriteRenderer.flipX = false;

        animator.SetFloat("MoveX", stateDirection.x);
        animator.SetFloat("MoveY", stateDirection.y);
    }

    public void IsMoving(bool isMoving) { animator.SetBool("IsMoving", isMoving); }

    public Vector3 AnimationDirection()
    {
        return new Vector3(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
    }
}
