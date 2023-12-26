using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIUniverseTravelToMain : MonoBehaviour
{
    public GameObject triggerObject;  // Reference to the object to check activation status

    void Update()
    {
        // Check if the player is inside the trigger area and pressing the "T" key
        if (Input.GetKeyDown(KeyCode.T) && IsTriggerObjectActive())
        {
            // Go to universe 1
            SceneManager.LoadScene("MainUniverse");
        }
    }

    bool IsTriggerObjectActive()
    {
        // Check if the trigger object is active
        return triggerObject.activeSelf;
    }

}
