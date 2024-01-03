using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{

    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;

    void OnMouseDown()
    {
        ToggleObjectsVisibility(objectsToShow, true);
        ToggleObjectsVisibility(objectsToHide, false);
    }

    void ToggleObjectsVisibility(GameObject[] objects, bool isVisible)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(isVisible);
        }
    }
}
