using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnCameraRotateLeftAction;
    public event EventHandler OnCameraRotateRightAction;

    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
        inputActions.Player.Enable();
        inputActions.Camera.Enable();
    }

    private void Start()
    {
        inputActions.Camera.RotateLeft.performed += RotateLeft_performed;
        inputActions.Camera.RotateRight.performed += RotateRight_performed;
    }

    private void RotateLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCameraRotateLeftAction?.Invoke(this, EventArgs.Empty);
    }

    private void RotateRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCameraRotateRightAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        return inputVector.normalized;
    }
}
