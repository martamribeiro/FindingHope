using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIUniverseTravelToMain : MonoBehaviour
{

    void Update()
    {
        // Check if the player is pressing the "T" key
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Go to universe 1
            SceneManager.LoadScene("MainUniverse");
        }
    }

}
