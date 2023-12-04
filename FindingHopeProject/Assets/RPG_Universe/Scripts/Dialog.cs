using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class only serves to save the Dialog Lines of the Game object that utilizes it
/// This facilitates further use in the DialogManager (Utility class of some sorts)
/// </summary>
[System.Serializable]
public class Dialog
{
    [SerializeField] List<string> lines;

    public List<string> Lines
    {
        get { return lines; }
    }
}
