using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] private Transform bricks;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float startDisapearTime = 1.0f;
    [SerializeField] private float disapearTime = 2.0f;

    private float timeElapsed;
    private IEnumerator destroySelfCoroutine;
    private IEnumerator cleanUpCoroutine;

    private void Start()
    {
        destroySelfCoroutine = DestroySelf();
        cleanUpCoroutine = CleanUp();
    }

    public Transform GetInteractionPoint()
    {
        return interactionPoint;
    }

    public void Break()
    {
        for (int i = 0; i < bricks.childCount; i++)
        {
            Transform brick = bricks.GetChild(i);

            brick.GetComponent<Rigidbody>().isKinematic = false;
        }

        StartCoroutine(destroySelfCoroutine);
    }

    private IEnumerator DestroySelf()
    {
        yield return StartCoroutine(cleanUpCoroutine);

        Destroy(gameObject);
    }

    private IEnumerator CleanUp()
    {
        Vector3 originalScale = bricks.GetChild(0).localScale;

        while (timeElapsed < disapearTime)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= startDisapearTime)
            {
                GetComponent<BoxCollider>().enabled = false;
                GetComponent<NavMeshObstacle>().enabled = false;

                float elapsedPercentage = timeElapsed / disapearTime;
                elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);

                for (int i = 0; i < bricks.childCount; i++)
                {
                    Transform brick = bricks.GetChild(i);

                    brick.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedPercentage);
                }
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
}
