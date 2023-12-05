using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject[] activatableGameObjectArray;

    private IActivatable[] activatableObjectArray;

    private void Start()
    {
        activatableObjectArray = new IActivatable[activatableGameObjectArray.Length];

        for(int i = 0; i < activatableGameObjectArray.Length; i++)
        {
            activatableObjectArray[i] = activatableGameObjectArray[i].GetComponent<IActivatable>();
        }
    }

    public void Interact(Player player)
    {
        foreach (IActivatable activatableObject in activatableObjectArray)
        {
            activatableObject.Toggle();
        }
    }
}
