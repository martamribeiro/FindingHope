using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IActivatable
{
    [SerializeField] private WaypointPath waypointPath;
    [SerializeField] private float speed;
    [SerializeField] private bool isActive = false;

    private int targetWaypointIndex;

    private Transform previousWaypoint;
    private Transform targetWaypoint;

    private float timeToWaypoint;
    private float timeElapsed;

    private bool waitingToDeactivate = false;

    private void Start()
    {
        TargetNextWaypoint();
    }

    private void FixedUpdate()
    {
        if (!isActive)
            return;

        timeElapsed += Time.deltaTime;

        float elapsedPercentage = timeElapsed / timeToWaypoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(previousWaypoint.position, targetWaypoint.position, elapsedPercentage);
        transform.rotation = Quaternion.Lerp(previousWaypoint.rotation, targetWaypoint.rotation, elapsedPercentage);

        if (elapsedPercentage >= 1)
        {
            TargetNextWaypoint();

            if (waitingToDeactivate)
            {
                isActive = false;
                waitingToDeactivate = false;
            }
        }
    }

    private void TargetNextWaypoint()
    {
        previousWaypoint = waypointPath.GetWaypoint(targetWaypointIndex);
        targetWaypointIndex = waypointPath.GetNextWaypointIndex(targetWaypointIndex);
        targetWaypoint = waypointPath.GetWaypoint(targetWaypointIndex);

        timeElapsed = 0.0f;

        float distanceToWaypoint = Vector3.Distance(previousWaypoint.position, targetWaypoint.position);
        timeToWaypoint = distanceToWaypoint / speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!(other.TryGetComponent(out Box box) && box.HasParent())) {
            other.transform.SetParent(null);
        }
    }

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            waitingToDeactivate = false;
        }
    }

        public void Deactivate()
    {
        if (isActive) 
            waitingToDeactivate = true;
    }

    public void Toggle()
    {
        if (!isActive) 
            isActive = true;
        else
            waitingToDeactivate = true;

    }
}
