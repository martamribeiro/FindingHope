using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PasswordInsert : MonoBehaviour
{
    public GameObject triggerObject;  // Reference to the object to check activation status
    public GameObject[] objectsToShowRight;  // Array of objects to show
    public GameObject[] objectsToHideRight;  // Array of objects to hide
    public GameObject[] objectsToShowWrong;  // Array of objects to show

    void Update()
    {
        // Check if the player is inside the trigger area and pressing the "B" key
        if (Input.GetKeyDown(KeyCode.B) && IsTriggerObjectActive())
        {
            // Toggle visibility of objects
            ToggleObjectsVisibility(objectsToShowRight, true);  // Show objects
            ToggleObjectsVisibility(objectsToHideRight, false);  // Hide objects
        }
        // Check if the player is inside the trigger area and pressing the "G" key
        if (Input.GetKeyDown(KeyCode.G) && IsTriggerObjectActive())
        {
            // Toggle visibility of objects
            ToggleObjectsVisibility(objectsToShowWrong, true);  // Show objects
        }
        // Check if the player is inside the trigger area and pressing the "R" key
        if (Input.GetKeyDown(KeyCode.R) && IsTriggerObjectActive())
        {
            // Toggle visibility of objects
            ToggleObjectsVisibility(objectsToShowWrong, true);  // Show objects
        }
    }

    bool IsTriggerObjectActive()
    {
        // Check if the trigger object is active
        return triggerObject.activeSelf;
    }

    void ToggleObjectsVisibility(GameObject[] objects, bool isVisible)
    {
        // Toggle visibility of each object in the array
        foreach (GameObject obj in objects)
        {
            obj.SetActive(isVisible);
        }
    }

}
