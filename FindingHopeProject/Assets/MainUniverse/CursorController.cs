using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour
{

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        // Handle cursor locking/unlocking
        HandleCursorLocking();
    }

    /// <summary>
    ///     Unlock the cursor when the TAB key is pressed, Re-lock the cursor when the left mouse button is pressed
    /// </summary>
    private void HandleCursorLocking()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) LockCursor();
    }

    private static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}