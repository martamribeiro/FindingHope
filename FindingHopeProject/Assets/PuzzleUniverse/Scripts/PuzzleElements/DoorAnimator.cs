using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimator : MonoBehaviour
{
    private const string OPEN = "Open";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        animator.SetBool(OPEN, true);
    }

    public void CloseDoor()
    {
        animator.SetBool(OPEN, false);
    }
}
