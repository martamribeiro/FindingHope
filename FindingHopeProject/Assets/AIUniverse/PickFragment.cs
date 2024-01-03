using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickFragment : MonoBehaviour
{
    public GameObject triggerObject;  // Reference to the object to check activation status
    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;

    void Update()
    {
        // Check if the player is pressing the "T" key
        if (Input.GetKeyDown(KeyCode.F) && IsTriggerObjectActive())
        {
            ToggleObjectsVisibility(objectsToShow, true);
            ToggleObjectsVisibility(objectsToHide, false);
        }
    }

    void ToggleObjectsVisibility(GameObject[] objects, bool isVisible)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(isVisible);
        }
    }

    bool IsTriggerObjectActive()
    {
        // Check if the trigger object is active
        return triggerObject.activeSelf;
    }

}
