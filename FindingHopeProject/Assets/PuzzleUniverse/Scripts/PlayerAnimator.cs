using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        UpdateAnimations();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        animator.SetBool(IS_WALKING, Player.Instance.IsWalking());
        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
    }
}
