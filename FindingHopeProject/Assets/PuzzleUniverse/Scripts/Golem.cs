using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : MonoBehaviour
{
    private enum State
    {
        Follow,
        StandBy,
        Moving,
        Carrying,
        Interacting,
    }

    [SerializeField] Camera camera;
    [SerializeField] GameInput gameInput;

    private NavMeshAgent navMeshAgent;
    private Player playerInstance;

    private BreakableWall targetBreakableWall;

    private State state = State.StandBy;

    private void Update()
    {
        switch (state)
        {
            case State.StandBy:
                break;
            case State.Follow:
                FollowAction();
                break;
            case State.Moving:
                if (Vector3.Distance(transform.position, navMeshAgent.destination) < 0.05f)
                {
                    navMeshAgent.isStopped = true;
                    state = State.StandBy;
                }
                break;
            case State.Carrying:
                break;
            case State.Interacting:
                InteractAction();
                break;
        }
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        gameInput.OnGolemMoveAction += GameInput_OnGolemMoveAction;

        playerInstance = Player.Instance;
    }

    private void GameInput_OnGolemMoveAction(object sender, System.EventArgs e)
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        if (MousePosition.GetRaycastHitFromMouseInput(camera, out RaycastHit raycastHit))
        {
            if (raycastHit.collider.transform.TryGetComponent<Player>(out _))
            {
                state = State.Follow;
            }
            else if (raycastHit.collider.transform.TryGetComponent<BreakableWall>(out BreakableWall breakableWall))
            {
                state = State.Interacting;

                targetBreakableWall = breakableWall;
                navMeshAgent.destination = breakableWall.GetInteractionPoint().position;

                if (navMeshAgent.isStopped)
                    navMeshAgent.isStopped = false;
            }
            else
            {
                navMeshAgent.destination = raycastHit.point;
                state = State.Moving;

                if (navMeshAgent.isStopped)
                    navMeshAgent.isStopped = false;
            }
        }
    }

    private void FollowAction()
    {
        float wantedDistanceToPlayer = 3.0f;

        if (Vector3.Distance(transform.position, playerInstance.transform.position) > wantedDistanceToPlayer)
        {
            navMeshAgent.destination = playerInstance.transform.position;

            if (navMeshAgent.isStopped)
                navMeshAgent.isStopped = false;
        }
        else
        {
            navMeshAgent.isStopped = true;
        }
    }

    private void InteractAction()
    {
        if (Vector3.Distance(transform.position, navMeshAgent.destination) < 0.5f) {
            targetBreakableWall.Break();
            state = State.StandBy;
        }
    }
}
