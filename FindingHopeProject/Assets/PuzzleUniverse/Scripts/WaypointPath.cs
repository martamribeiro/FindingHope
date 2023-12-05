using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    private enum TraversingMode
    {
        Cycle,
        Mirror
    }

    [SerializeField] TraversingMode traversingMode;

    private bool mirror = false;

    public Transform GetWaypoint(int waypointIndex)
    {
        return transform.GetChild(waypointIndex);
    }

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        return traversingMode switch
        {
            TraversingMode.Cycle => GetNextWaypoingIndexCycle(currentWaypointIndex),
            TraversingMode.Mirror => GetNextWaypoingIndexMirror(currentWaypointIndex),
            _ => currentWaypointIndex,
        };
    }

    private int GetNextWaypoingIndexCycle(int currentWaypointIndex)
    {
        int nextWaypointIndex = currentWaypointIndex + 1;

        if (nextWaypointIndex == transform.childCount)
        {
            nextWaypointIndex = 0;
        }

        return nextWaypointIndex;
    }

    private int GetNextWaypoingIndexMirror(int currentWaypointIndex)
    {
        int nextWaypointIndex;

        if (mirror)
        {
            nextWaypointIndex = currentWaypointIndex - 1;

            if (nextWaypointIndex == -1)
            {
                nextWaypointIndex = 1;
                mirror = false;
            }

            return nextWaypointIndex;
        }

        nextWaypointIndex = currentWaypointIndex + 1;

        if (nextWaypointIndex == transform.childCount)
        {
            nextWaypointIndex = currentWaypointIndex - 1;
            mirror = true;
        }

        return nextWaypointIndex;
    }
}
