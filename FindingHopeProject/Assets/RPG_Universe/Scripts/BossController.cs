using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] float waitTime = 5f;

    public void Interact()
    {
        StartCoroutine(HandleBossInteraction());
    }

    IEnumerator HandleBossInteraction()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        yield return new WaitForSeconds(waitTime);
        Destroy(this.gameObject);
        SceneManager.LoadScene("TurnBaseBattle");
    }
}
