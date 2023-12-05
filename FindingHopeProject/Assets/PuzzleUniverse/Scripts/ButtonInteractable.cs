using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject activatableGameObject;

    private IActivatable activatableObject;

    private void Start()
    {
        activatableObject = activatableGameObject.GetComponent<IActivatable>();
    }

    public void Interact(Player player)
    {
        activatableObject.Toggle();
    }
}
