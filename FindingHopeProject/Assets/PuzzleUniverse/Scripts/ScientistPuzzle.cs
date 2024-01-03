using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistPuzzle : MonoBehaviour, IInteractable
{
    [SerializeField]
    GameObject pageUIContent;

    private bool WasPickedUp = false;

    public void Interact(Player player)
    {
        if (!WasPickedUp)
        {
            pageUIContent?.SetActive(true);

            WasPickedUp = true;
        }
    }
}
