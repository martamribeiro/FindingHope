using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] GameInput gameInput;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        gameInput.OnGolemMoveAction += GameInput_OnGolemMoveAction;
    }

    private void GameInput_OnGolemMoveAction(object sender, System.EventArgs e)
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        if (MousePosition.GetRaycastHitFromMouseInput(camera, out RaycastHit raycastHit))
        {
            navMeshAgent.destination = raycastHit.point;
        }
    }
}
