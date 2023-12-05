using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPressurePlate : PressurePlate
{
    [SerializeField] MultiPressurePlate[] multiPressurePlateArray;

    private int numberOfEntitiesPressing = 0;
    private bool isActive = false;

    private void Start()
    {
        activatableObject = activatableGameObject.GetComponent<IActivatable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        numberOfEntitiesPressing++;

        if (numberOfEntitiesPressing == 1)
        {
            pressurePlateAnimator.SteppedOn();
            isActive = true;

            bool shouldActivate = true;

            foreach (MultiPressurePlate multiPressurePlate in multiPressurePlateArray)
            {
                if (!multiPressurePlate.IsAtivated())
                {
                    shouldActivate = false;
                    break;
                }
            }

            if (shouldActivate)
                activatableObject.Activate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        numberOfEntitiesPressing--;

        if (numberOfEntitiesPressing == 0)
        {
            pressurePlateAnimator.SteppedOff();
            isActive = false;

            bool shouldDeactivate = false;

            foreach (MultiPressurePlate multiPressurePlate in multiPressurePlateArray)
            {
                if (multiPressurePlate.IsAtivated())
                {
                    shouldDeactivate = true;
                    break;
                }
            }

            if (shouldDeactivate)
                activatableObject.Deactivate();
        }
    }

    public bool IsAtivated() 
    {
        return isActive; 
    }
}
