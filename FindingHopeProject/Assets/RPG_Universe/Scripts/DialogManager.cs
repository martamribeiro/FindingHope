using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages Dialog that is included in the different NPC (in this case through the BossController)
/// Handles message showing in coroutines. Npc's can have multiple lines of text in which the manager will scroll through each of them individually
/// I also create a Instance of the class so i can easily incorporated into other necessary scripts.
/// </summary>
public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    
    public static DialogManager Instance { get; private set; }

    // Markers for the script to know when to close the dialogbox or to keep it open
    public event Action OnShowDialog;
    public event Action OnHideDialog;

    // Dialog Text Box State variables
    private int currentLine = 0;
    private Dialog dialog;
    private bool isTyping = false;

    public void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Handles the current dialog that it has, looping through the Dialog.Lines array until all messages are displayed.
    /// Messages always wait for the KeyCode (this can be changed depending on needs).
    /// Types each lines at a time keeps track of what is written
    /// </summary>
    /// <exception cref="Exception">Throws exception when dialog is not included</exception>
    public void HandleUpdate()
    {
        if (dialog == null) throw new Exception("Dialog not found!");

        if (Input.GetKeyDown(KeyCode.E) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeLine(dialog.Lines[currentLine]));
            } else
            {
                dialogBox.SetActive(false);
                OnHideDialog.Invoke();
                currentLine = 0;
            }
        }
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        if (dialog == null) throw new Exception("Dialog not found");
        yield return new WaitForEndOfFrame();

        // Invoke functions function like signals (used in the GameController to manage Game States)
        OnShowDialog?.Invoke();
        this.dialog = dialog;

        dialogBox.SetActive(true);
        StartCoroutine(TypeLine(dialog.Lines[0]));
    }

    public IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond); // unnecessary but is cool seeing the letters written one by one
        }
        isTyping = false;
    }
}
