using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Convai.Scripts;

public class PasswordInsert : MonoBehaviour
{
    public GameObject[] objectsToShowRight;
    public GameObject[] objectsToHideRight;
    public GameObject[] objectsToShowWrong;
    public GameObject[] objectsToHideWrong;
    public Button[] buttons;
    private Button correctButton;

    public GameObject[] hideBlueRight;
    public GameObject[] showBlueRight;
    public GameObject[] hideRedRight;
    public GameObject[] showRedRight;
    public GameObject[] hideGreenRight;
    public GameObject[] showGreenRight;

    /*public GameObject blueConvaiNPCObject;  // Reference to the Convai NPC Blue Robot GameObject
    private ConvaiNPC blueConvaiNPC;  // Reference to the ConvaiNPC script on the GameObject
    public GameObject greenConvaiNPCObject;
    private ConvaiNPC greenConvaiNPC;
    public GameObject redConvaiNPCObject;
    private ConvaiNPC redConvaiNPC;*/

    private int incorrectPressCount = 0;

    void Start()
    {
        // Initialize correctButton to the initial correct button (BlueButton)
        correctButton = Array.Find(buttons, b => b.name == "BlueButton");

        // Get the ConvaiNPC script component on the Convai NPC Blue Robot GameObject
        /*blueConvaiNPC = blueConvaiNPCObject.GetComponent<ConvaiNPC>();
        greenConvaiNPC = greenConvaiNPCObject.GetComponent<ConvaiNPC>();
        redConvaiNPC = redConvaiNPCObject.GetComponent<ConvaiNPC>();*/

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => ButtonClicked(button));
        }
    }

    void ButtonClicked(Button clickedButton)
    {
        if (IsCorrectButton(clickedButton))
        {
            ToggleObjectsVisibility(objectsToShowRight, true);
            ToggleObjectsVisibility(objectsToHideRight, false);
        }
        else
        {
            ToggleObjectsVisibility(objectsToShowWrong, true);
            ToggleObjectsVisibility(objectsToHideWrong, false);

            // Schedule a delayed hide after 5 seconds
            Invoke("DelayedHide", 1f);

            UpdateButtons();
        }
    }

    void DelayedHide()
    {
        // Hide the wrong answer objects after the delay
        ToggleObjectsVisibility(objectsToShowWrong, false);
    }

    bool IsCorrectButton(Button button)
    {
        return button == correctButton;
    }

    void UpdateButtons()
    {
        incorrectPressCount++;

        if (incorrectPressCount == 1)
        {
            // Change the correct button to GreenButton
            correctButton = Array.Find(buttons, b => b.name == "GreenButton");
            // Change the Blue Character ID
            /*blueConvaiNPC.characterID = "8111464e-aa39-11ee-aed7-42010a40000f";
            greenConvaiNPC.characterID = "3cbe3736-aa39-11ee-809d-42010a40000f";
            redConvaiNPC.characterID = "bbc45ca4-aa39-11ee-b8e5-42010a40000f";*/

            ToggleObjectsVisibility(hideGreenRight, false);
            ToggleObjectsVisibility(showGreenRight, true);

        }
        else if (incorrectPressCount == 2)
        {
            // Change the correct button to RedButton
            correctButton = Array.Find(buttons, b => b.name == "RedButton");
            // Change the Blue Character ID back
            /*blueConvaiNPC.characterID = "0691a5a8-aa39-11ee-aad8-42010a40000f";
            greenConvaiNPC.characterID = "9fc9d07e-aa39-11ee-aed7-42010a40000f";
            redConvaiNPC.characterID = "bbc45ca4-aa39-11ee-b8e5-42010a40000f";*/

            ToggleObjectsVisibility(hideRedRight, false);
            ToggleObjectsVisibility(showRedRight, true);

        }
        else if (incorrectPressCount == 3)
        {
            // Reset to the initial state, where BlueButton is the correct answer
            correctButton = Array.Find(buttons, b => b.name == "BlueButton");
            // Change the Blue Character ID back
            /*blueConvaiNPC.characterID = "0691a5a8-aa39-11ee-aad8-42010a40000f";
            greenConvaiNPC.characterID = "3cbe3736-aa39-11ee-809d-42010a40000f";
            redConvaiNPC.characterID = "62a4028c-aa39-11ee-b8f4-42010a40000f";*/

            ToggleObjectsVisibility(hideBlueRight, false);
            ToggleObjectsVisibility(showBlueRight, true);

            incorrectPressCount = 0;
        }
    }

    void ToggleObjectsVisibility(GameObject[] objects, bool isVisible)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(isVisible);
        }
    }
}
