using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughSync : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float appearSpeed = 2.0f;

    private static int posID = Shader.PropertyToID("_Player_Position");
    private static int sizeID = Shader.PropertyToID("_Size");
    private float size = 0f;


    void Update()
    {
        Vector3 direction = camera.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction.normalized);

        if (Physics.Raycast(ray, 20, mask))
        {
            if (size != 1.0f)
            {
                size = Mathf.Lerp(size, 1.0f, appearSpeed * Time.deltaTime);

                if (size > 0.95f)
                    size = 1.0f;
            }
        }
        else
        {
            if (size != 0f)
            {
                size = Mathf.Lerp(size, 0.0f, appearSpeed * Time.deltaTime);

                if (size < 0.05)
                    size = 0.0f;
            }
        }

        material.SetFloat(sizeID, size);

        Vector3 playerHeightOffset = new Vector3(0f, 1f, 0f);
        Vector3 view = camera.WorldToViewportPoint(transform.position + playerHeightOffset);
        material.SetVector(posID, view);
    }
}
