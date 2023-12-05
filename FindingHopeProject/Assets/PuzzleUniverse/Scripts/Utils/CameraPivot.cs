using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraPivot : MonoBehaviour
{
    private enum RotateDirection
    {
        Right,
        Left,
    }

    [SerializeField] GameInput gameInput;
    [SerializeField] private float rotationSpeed = 3.0f;

    private Quaternion targetRotation;
    private IEnumerator rotationCoroutine;

    private void Start()
    {
        gameInput.OnCameraRotateLeftAction += GameInput_OnCameraRotateLeftAction;
        gameInput.OnCameraRotateRightAction += GameInput_OnCameraRotateRightAction;

        targetRotation = transform.rotation;
    }

    private void GameInput_OnCameraRotateRightAction(object sender, System.EventArgs e)
    {
        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = RotateCameraPivot(RotateDirection.Right);
        StartCoroutine(rotationCoroutine);
    }

    private void GameInput_OnCameraRotateLeftAction(object sender, System.EventArgs e)
    {
        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = RotateCameraPivot(RotateDirection.Left);
        StartCoroutine(rotationCoroutine);
    }

    private IEnumerator RotateCameraPivot(RotateDirection rotateDirection)
    {
        if (rotateDirection == RotateDirection.Left)
        {
            targetRotation *= Quaternion.Euler(Vector3.up * 45f);
        }
        else if (rotateDirection == RotateDirection.Right) 
        {
            targetRotation *= Quaternion.Euler(Vector3.up * -45f);
        }

        while (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }
}
