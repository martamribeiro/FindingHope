using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsCollisionHandler : MonoBehaviour
{
    public GameObject objectB;
    public GameObject[] objectsToShow;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the triggering object is Object B
        if (other.gameObject == objectB)
        {
            // Show certain objects X
            ShowObjectsX();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the triggering object is Object B
        if (other.gameObject == objectB)
        {
            // Hide certain objects X
            HideObjectsX();
        }
    }

    void ShowObjectsX()
    {
        foreach (GameObject objX in objectsToShow)
        {
            objX.SetActive(true);
        }
    }

    void HideObjectsX()
    {
        foreach (GameObject objX in objectsToShow)
        {
            objX.SetActive(false);
        }
    }
}
