using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class RPGButton : MonoBehaviour
{
    void Start()
    {
        string btn = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(() => ManageAction(btn));
    }

    private void ManageAction(string btn)
    {
        switch (btn)
        {
            // manage attack
            case "AttackBtn":
                break;

            // manage bag system
            case "BagBtn":
                break;

            // manage run 
            case "RunBtn":
                break;
        }
    }
}
