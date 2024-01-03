using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    private void Start()
    {
        ActivateFoundPages();
    }

    private void ActivateFoundPages()
    {
        // Get the list of found page names from the PageManager
        List<string> foundPageNames = PageManager.Instance.GetFoundPageNames();

        // Iterate through child parts
        foreach (Transform contentTransform in transform)
        {
            // Iterate through child pages in each part
            foreach (Transform partTransform in contentTransform)
            {

                if(partTransform.gameObject.name == "Part1" || partTransform.gameObject.name == "Part2")
                {
                    foreach (Transform pageTransform in partTransform)
                    {
                        // Get the page component
                        Page page = pageTransform.GetComponent<Page>();

                        if (page != null && foundPageNames.Contains(pageTransform.gameObject.name))
                        {
                            // Activate the page if it's found
                            pageTransform.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}
