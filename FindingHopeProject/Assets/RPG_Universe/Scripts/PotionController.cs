using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] string potionName;

    public void Interact()
    {
        StartCoroutine(ObtainPotion());
    }

    IEnumerator ObtainPotion()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        yield return new WaitForSeconds(2.5f);
        Destroy(this.gameObject);
    }
}
