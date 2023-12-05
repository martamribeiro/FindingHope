using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoxParentObject
{
    public void GrabBox(Box box);

    public void DropBox();

    public Transform GetGrabPositionTransform();
}
