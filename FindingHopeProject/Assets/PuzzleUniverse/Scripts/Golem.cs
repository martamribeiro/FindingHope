using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class Golem : MonoBehaviour, IBoxParentObject, IInteractable
{
    public enum State
    {
        Inactive,
        Follow,
        StandBy,
        Moving,
        Carrying,
        Interacting,
    }

    [SerializeField] Camera camera;
    [SerializeField] GameInput gameInput;

    [SerializeField] Transform boxGrabPosition;
    [SerializeField] GolemAnimator animator;

    [Header("Raycast Settings")]
    [SerializeField] List<LayerMask> raycastLayerMaskList;
    [SerializeField] float maxDistance;

    private Box grabbedBox;

    private LayerMask raycastLayerMask;
    private NavMeshAgent navMeshAgent;
    private Player playerInstance;

    private IGolemInteractable targetInteractable;

    private State state = State.Inactive;

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
                CarryingAction();
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

        foreach (LayerMask layerMask in raycastLayerMaskList)
            raycastLayerMask |= layerMask;
    }

    private void GameInput_OnGolemMoveAction(object sender, System.EventArgs e)
    {
        if (state != State.Inactive) 
            PerformMovement();
    }

    private void PerformMovement()
    {
        if (MousePosition.GetRaycastHitFromMouseInput(camera, out RaycastHit raycastHit, maxDistance, raycastLayerMask))
        {
            if (raycastHit.collider.transform.TryGetComponent<Player>(out _))
            {
                state = State.Follow;
            }
            else if (raycastHit.collider.transform.TryGetComponent<IGolemInteractable>(out IGolemInteractable interactable))
            {
                state = State.Interacting;

                targetInteractable = interactable;
                navMeshAgent.destination = interactable.GetInteractionPoint().position;

                if (navMeshAgent.isStopped)
                    navMeshAgent.isStopped = false;
            }
            else
            {
                navMeshAgent.destination = raycastHit.point;

                if (state != State.Carrying)
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
        if (Vector3.Distance(transform.position, navMeshAgent.destination) < 0.5f || (targetInteractable is Box && Vector3.Distance(transform.position, navMeshAgent.destination) < 2f)) {
            state = State.StandBy;
            targetInteractable.Interact(this);

            animator.TriggerInteractionAnimation();
        }
    }

    private void CarryingAction()
    {
        if (navMeshAgent.isStopped)
            return;

        if (Vector3.Distance(transform.position, navMeshAgent.destination) < 1f)
            DropBox();
    }

    public void GrabBox(Box box)
    {
        navMeshAgent.isStopped = true;

        grabbedBox = box;
        state = State.Carrying;

        grabbedBox.GetComponent<Rigidbody>().useGravity = false;
        grabbedBox.GetComponent<NavMeshObstacle>().enabled = false;

    }

    public void DropBox()
    {
        grabbedBox.RemoveParent();

        grabbedBox.GetComponent<Rigidbody>().useGravity = true;
        grabbedBox.GetComponent<NavMeshObstacle>().enabled = true;

        grabbedBox.transform.position = navMeshAgent.destination;

        grabbedBox = null;
        state = State.StandBy;
        navMeshAgent.isStopped = true;
    }

    public Transform GetGrabPositionTransform()
    {
        return boxGrabPosition;
    }

    public State GetCurrentState()
    {
        return state;
    }

    public void Interact(Player player)
    {
        state = State.StandBy;
        animator.TriggerActiveAnimation();
    }
}
