using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RPGUniverseTravelToMain : MonoBehaviour
{
    void Update()
    {
        // Check if the player is inside the trigger area and pressing the "T" key
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Go to universe 1
            SceneManager.LoadScene("MainUniverse");
        }
    }

}
