using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnCameraRotateLeftAction;
    public event EventHandler OnCameraRotateRightAction;

    public event EventHandler OnPlayerInteractAction;

    public event EventHandler OnPlayerJumpAction;

    public event EventHandler OnPlayerRunAction;

    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
        inputActions.Player.Enable();
        inputActions.Camera.Enable();
        inputActions.Golem.Enable();
    }

    private void Start()
    {
        #region CameraInputs
        inputActions.Camera.RotateLeft.performed += CameraRotateLeft_performed;
        inputActions.Camera.RotateRight.performed += CameraRotateRight_performed;
        #endregion

        #region PlayerInputs
        inputActions.Player.Interact.performed += PlayerInteractAction_performed;

        inputActions.Player.Jump.started += PlayerJumpAction_started;

        inputActions.Player.Run.started += PlayerRun_action;
        inputActions.Player.Run.canceled += PlayerRun_action;
        #endregion

        #region GolemInputs
        inputActions.Golem.Move.performed += GolemMoveAction_performed;
        #endregion
    }

    private void GolemMoveAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (MousePosition.GetRaycastHitFromMouseInput(Camera.main, out RaycastHit hit))
        {
            Debug.Log(hit.point);
        }
    }

    private void PlayerInteractAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerRun_action(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerRunAction?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerJumpAction_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void CameraRotateLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCameraRotateLeftAction?.Invoke(this, EventArgs.Empty);
    }

    private void CameraRotateRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCameraRotateRightAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        return inputVector.normalized;
    }
}
