using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScientistPuzzle : MonoBehaviour, IInteractable
{
    [SerializeField]
    GameObject pageUIContent;

    [SerializeField]
    GameObject page;

    private bool WasPickedUp = false;

    public void Interact(Player player)
    {
        if (!WasPickedUp)
        {
            pageUIContent?.SetActive(true);

            page?.SetActive(true);

            WasPickedUp = true;
        }
    }
}
