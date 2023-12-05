using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateAnimator : MonoBehaviour
{
    private const string STEPPED_ON = "SteppedOn";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SteppedOn()
    {
        animator.SetBool(STEPPED_ON, true);
    }

    public void SteppedOff()
    {
        animator.SetBool(STEPPED_ON, false);
    }
}
