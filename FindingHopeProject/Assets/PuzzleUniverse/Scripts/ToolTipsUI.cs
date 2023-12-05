using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipsUI : MonoBehaviour
{
    [SerializeField] private RectTransform basis;
    [SerializeField] private RectTransform movingUI;
    [SerializeField] private Camera camera;
    [SerializeField] private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        MoveUI();   
    }

    private void MoveUI()
    {
        Vector3 pos = Input.mousePosition + offset;
        pos.z = basis.position.z;

        movingUI.position = camera.ScreenToWorldPoint(pos);
    }
}
