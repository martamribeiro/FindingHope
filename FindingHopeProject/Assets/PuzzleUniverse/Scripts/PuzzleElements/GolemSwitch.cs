using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSwitch : MonoBehaviour, IGolemInteractable
{
    [SerializeField] private GameObject activatableGameObject;
    [SerializeField] private GolemSwitchAnimator golemSwitchAnimator;
    [SerializeField] private Transform interactionPoint;

    private IActivatable activatableObject;
    private bool isEnabled = false;
    
    private void Start()
    {
        activatableObject = activatableGameObject.GetComponent<IActivatable>();
    }

    public Transform GetInteractionPoint()
    {
        return interactionPoint;
    }

    public void Interact(Golem golem)
    {
        if (!isEnabled)
        {
            activatableObject?.Activate();
            golemSwitchAnimator.EnableSwitch();
        }
        else
        {
            activatableObject?.Deactivate();
            golemSwitchAnimator.DisableSwitch();
        }

        isEnabled = !isEnabled;
    }
}
