using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordInsert : MonoBehaviour
{
    public GameObject[] objectsToShowRight;  // Array of objects to show
    public GameObject[] objectsToHideRight;  // Array of objects to hide
    public GameObject[] objectsToShowWrong;  // Array of objects to show
    public GameObject[] objectsToHideWrong;  // Array of objects to hide
    public Button correctButton;  // Reference to the correct UI button
    public Button[] incorrectButtons;  // Array of references to the incorrect UI buttons

    public void Update()
    {
        // Assign functions to be called when buttons are clicked
        correctButton.onClick.AddListener(CorrectButtonClicked);
        foreach (Button button in incorrectButtons)
        {
            button.onClick.AddListener(IncorrectButtonClicked);
        }
    }

    public void CorrectButtonClicked()
    {
        // Toggle visibility of objects for the correct answer
        ToggleObjectsVisibility(objectsToShowRight, true);
        ToggleObjectsVisibility(objectsToHideRight, false);
    }

    public void IncorrectButtonClicked()
    {
        // Toggle visibility of objects for the incorrect answer
        ToggleObjectsVisibility(objectsToShowWrong, true);
        ToggleObjectsVisibility(objectsToHideWrong, false);
    }

    public void ToggleObjectsVisibility(GameObject[] objects, bool isVisible)
    {
        // Toggle visibility of each object in the array
        foreach (GameObject obj in objects)
        {
            obj.SetActive(isVisible);
        }
    }

}
