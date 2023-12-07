using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MousePosition
{
    public static bool GetRaycastHitFromMouseInput(Camera camera, out RaycastHit raycastHit)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out raycastHit))
        {
            return true;
        } 

        return false;
    }

    public static bool GetRaycastHitFromMouseInput(Camera camera, out RaycastHit raycastHit, float maxDistance, LayerMask layerMask)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out raycastHit, maxDistance, layerMask))
        {
            return true;
        }

        return false;
    }
}
