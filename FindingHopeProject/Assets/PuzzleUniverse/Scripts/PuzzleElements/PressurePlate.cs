using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] protected GameObject activatableGameObject;
    [SerializeField] protected PressurePlateAnimator pressurePlateAnimator;

    protected IActivatable activatableObject;
    protected int totalNumberOfEntities = 0;

    private void Start()
    {
        activatableObject = activatableGameObject.GetComponent<IActivatable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        totalNumberOfEntities++;

        if (totalNumberOfEntities == 1)
        {
            pressurePlateAnimator.SteppedOn();
            activatableObject.Toggle();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        totalNumberOfEntities--;

        if (totalNumberOfEntities == 0)
        {
            pressurePlateAnimator.SteppedOff();
            activatableObject.Toggle();
        }
    }
}
