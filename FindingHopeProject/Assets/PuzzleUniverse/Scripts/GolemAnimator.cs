using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_CARRYING = "IsCarrying";
    private const string INTERACT = "Interact";

    [SerializeField] 
    private Golem golem;
    
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
        animator.SetBool(IS_WALKING, golem.GetCurrentState() == Golem.State.Moving || golem.GetCurrentState() == Golem.State.Follow);
        animator.SetBool(IS_CARRYING, golem.GetCurrentState() == Golem.State.Carrying);
    }

    public void TriggerInteractionAnimation()
    {
        animator.SetTrigger(INTERACT);
    }
}
