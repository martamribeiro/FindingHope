using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBackMain : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    public void Interact()
    {
        StartCoroutine(PortalToMain());
    }

    IEnumerator PortalToMain()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MainUniverse");
    }
}
