using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSwitchAnimator : MonoBehaviour
{
    private const string SWITCH_ENABLED = "SwitchEnabled";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void EnableSwitch()
    {
        animator.SetBool(SWITCH_ENABLED, true);
    }

    public void DisableSwitch()
    {
        animator.SetBool(SWITCH_ENABLED, false);
    }
}
