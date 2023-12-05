using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IActivatable
{
    [SerializeField] private DoorAnimator doorAnimator;
    [SerializeField] private bool isOpen = false;

    private void Start()
    {
        if (isOpen)
            Activate();
        else
            Deactivate();
    }

    public void Activate()
    {
        doorAnimator.OpenDoor();
        isOpen = true;
    }

    public void Deactivate()
    {
        doorAnimator.CloseDoor();
        isOpen = false;
    }

    public void Toggle()
    {
        if (isOpen)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }
}
