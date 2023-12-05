using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState { FREEROAM, DIALOG, BATTLE}

/// <summary>
/// Controls current Game State 
/// Should be added into a empty GameObject that shelters this class (see scene for reference) 
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] Movement2D player;
    GameState state;

    private void Start()
    {
        // Change Game states according to invoke messages sent by the DialogManager
        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.DIALOG;
        };
        DialogManager.Instance.OnHideDialog += () =>
        {
            // Until turn based isnt developed
            if (state == GameState.DIALOG) state = GameState.FREEROAM;
        };
    }

    private void Update()
    {
        if (state == GameState.FREEROAM)
        {
            player.HandleUpdate();
        } 
        else if (state == GameState.DIALOG)
        {
            DialogManager.Instance.HandleUpdate();
        } 
        else if (state == GameState.BATTLE)
        {
            // to be developed
        }
    }
}
