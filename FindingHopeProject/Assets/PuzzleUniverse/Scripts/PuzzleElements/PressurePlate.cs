using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private GameObject activatableGameObject;
    [SerializeField] private PressurePlateAnimator pressurePlateAnimator;

    private IActivatable activatableObject;
    private int totalNumberOfEntities = 0;

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
