using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    private static PageManager _instance;
    public static PageManager Instance => _instance;

    // List of found page names
    private List<string> foundPageNames = new List<string>();

    private void Awake()
    {
        // Implementing the Singleton pattern
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to add a found page name
    public void AddFoundPage(string pageName)
    {
        if (!foundPageNames.Contains(pageName))
        {
            foundPageNames.Add(pageName);
        }
    }

    // Method to get the list of found page names
    public List<string> GetFoundPageNames()
    {
        return foundPageNames;
    }
}
