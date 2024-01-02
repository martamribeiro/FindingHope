using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIUniverseTravelToMain : MonoBehaviour
{

    public GameObject invisibleObject;
    public GameObject objectToHide;

    void Update()
    {
        if (!IsObjectInvisible())
        {
            HideObject();
        }
        else
        {
            ShowObject();
        }
        // Check if the player is pressing the "T" key
        if (Input.GetKeyDown(KeyCode.T) && IsObjectInvisible())
        {
            // Go to universe 1
            SceneManager.LoadScene("MainUniverse");
        }
    }

    bool IsObjectInvisible()
    {
        // Check if the object is inactive (invisible)
        return !invisibleObject.activeSelf;
    }

    void HideObject()
    {
        // Hide the specified object
        objectToHide.SetActive(false);
    }

    void ShowObject()
    {
        // Show the specified object
        objectToHide.SetActive(true);
    }

}
