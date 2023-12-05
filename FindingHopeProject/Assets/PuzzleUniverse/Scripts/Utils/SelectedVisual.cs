using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
    [SerializeField] private GameObject interactableObject;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private IInteractable interactable;

    private void Awake()
    {
        interactable = interactableObject.GetComponent<IInteractable>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<CharacterController>() != null)
        {
            Show();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<CharacterController>() != null)
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
            visualGameObject.SetActive(true);
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
            visualGameObject.SetActive(false);
    }
}
