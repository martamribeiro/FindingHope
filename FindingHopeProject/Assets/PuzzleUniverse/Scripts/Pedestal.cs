using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestal : MonoBehaviour, IInteractable
{
    [SerializeField]
    GameObject fragmentUIContent;

    [SerializeField]
    GameObject fragmentObject;

    private bool WasPickedUp = false;

    public void Interact(Player player)
    {
        if (!WasPickedUp)
        {
            fragmentUIContent?.SetActive(true);
            fragmentObject?.SetActive(false);

            WasPickedUp = true;
        }
    }
}
