using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UIElements;

public class Box : MonoBehaviour, IInteractable, IGolemInteractable
{
    [SerializeField] private SelectedVisual selectVisual;
    [SerializeField] private Transform interactionPoint;

    private bool hasParent = false;

    public void Interact(Golem golem)
    {
        receiveParent(golem);
    }

    public void Interact(Player player)
    {
        receiveParent(player);
    }

    private void receiveParent(IBoxParentObject parentObject)
    {
        if (hasParent)
            return;

        parentObject.GrabBox(this);

        transform.SetParent(parentObject.GetGrabPositionTransform());
        transform.position = parentObject.GetGrabPositionTransform().position;

        GetComponent<Rigidbody>().freezeRotation = true;

        selectVisual.gameObject.SetActive(false);

        hasParent = true;
    }

    public void RemoveParent()
    {
        hasParent = false;
        transform.SetParent(null);

        GetComponent<Rigidbody>().freezeRotation = false;

        selectVisual.gameObject.SetActive(true);
    }

    public Transform GetInteractionPoint()
    {
        return interactionPoint;
    }

    public bool HasParent()
    {
        return hasParent;
    }
}
