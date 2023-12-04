using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    public void Interact()
    {
        // This is an example of how the DialogManager is used to display dialog
        // You can have this scheme in any script you want
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
