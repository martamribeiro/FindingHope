using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectsCollisionHandler : MonoBehaviour
{
    public GameObject objectB;
    public GameObject[] objectsToShow;
    public TextMeshProUGUI textMeshPro;

    [SerializeField]
    private string customText = "Robot";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the triggering object is Object B
        if (other.gameObject == objectB)
        {
            // Show certain objects X
            ShowObjectsX();
        }

        // Change the content of TextMeshPro
        ChangeText(customText);
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the triggering object is Object B
        if (other.gameObject == objectB)
        {
            // Hide certain objects X
            HideObjectsX();
        }

        // Change the content of TextMeshPro
        ChangeText("Robot");
    }

    void ShowObjectsX()
    {
        foreach (GameObject objX in objectsToShow)
        {
            objX.SetActive(true);
        }
    }

    void HideObjectsX()
    {
        foreach (GameObject objX in objectsToShow)
        {
            objX.SetActive(false);
        }
    }

    void ChangeText(string newText)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = newText;
        }
    }
}
