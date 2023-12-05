using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatable
{
    public void Activate();
    
    public void Deactivate();

    public void Toggle();
}
