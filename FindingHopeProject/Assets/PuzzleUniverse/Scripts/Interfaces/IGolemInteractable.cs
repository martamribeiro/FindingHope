using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGolemInteractable
{
    public void Interact(Golem golem);

    public Transform GetInteractionPoint();
}
