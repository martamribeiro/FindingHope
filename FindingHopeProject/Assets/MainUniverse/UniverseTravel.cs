using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UniverseTravel : MonoBehaviour
{
    public GameObject triggerObject;  // Reference to the object to check activation status

    void Update()
    {
        // Check if the player is inside the trigger area and pressing the "1" key
        if (Input.GetKeyDown(KeyCode.Alpha1) && IsTriggerObjectActive())
        {
            // Go to universe 1
            SceneManager.LoadScene("AIUniverse");
        }
        // Check if the player is inside the trigger area and pressing the "2" key
        if (Input.GetKeyDown(KeyCode.Alpha2) && IsTriggerObjectActive())
        {
            // Go to universe 2
        }
        // Check if the player is inside the trigger area and pressing the "3" key
        if (Input.GetKeyDown(KeyCode.Alpha3) && IsTriggerObjectActive())
        {
            // Go to universe 3
        }
    }

    bool IsTriggerObjectActive()
    {
        // Check if the trigger object is active
        return triggerObject.activeSelf;
    }

}
