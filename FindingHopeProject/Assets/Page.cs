using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    private bool addedToFoundPages = false;

    private void Start()
    {
        // Check and update the found status during the initial setup
        CheckAndSetFoundStatus();
    }

    private void Update()
    {
        // Check and update the found status during each frame
        CheckAndSetFoundStatus();
    }

    // Method to check the found status and set activation accordingly
    private void CheckAndSetFoundStatus()
    {
        if (gameObject.activeSelf && !addedToFoundPages)
        {
            // Page is found, add it to the list if not already present
            AddToFoundPages();
        }
    }

    // Method to add the page to the foundPages list
    private void AddToFoundPages()
    {
        // Check if the PageManager exists
        PageManager pageManager = FindObjectOfType<PageManager>();
        if (pageManager != null)
        {
            // Add the page to foundPages
            pageManager.AddFoundPage(gameObject.name);
            addedToFoundPages = true; // Set the flag to true to indicate that the page has been added
        }
    }
}
