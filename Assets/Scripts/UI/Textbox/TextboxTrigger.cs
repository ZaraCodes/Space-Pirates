using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// [RequireComponent(typeof(BoxCollider2D))]
public class TextboxTrigger : MonoBehaviour
{
    #region Fields
    [SerializeField] private TextboxSequence dialogSequence;
    [SerializeField] private bool gameplayDialog;
    
    /// <summary>This array contains all the required progression flags that have to be set for this dialog to activate</summary>
    [SerializeField] private EProgressionFlag[] requirements;
    /// <summary>This arrac contains all progression flags that will prevent this dialog to be activated</summary>
    [SerializeField] private EProgressionFlag[] antiRequirements;

    [SerializeField] private UnityEvent OnDialogFinished;

    #endregion

    #region Methods
    public void LoadDialog()
    {
        foreach (var antiRequirement in antiRequirements)
        {
            if (ProgressionManager.Instance.Flags.Contains(antiRequirement))
            {
                return;
            }
        }
        foreach (var requirement in requirements)
        {
            if (!ProgressionManager.Instance.Flags.Contains(requirement))
            {
                return;
            }
        }
        var dialog = dialogSequence;
        if (ProgressionManager.Instance.ViewedDialogs.Contains(dialogSequence.ID))
        {
            if (!dialogSequence.Repeat && dialogSequence.SummaryDialog != null)
            {
                dialog = dialogSequence.SummaryDialog;
            }
            else return;
        }
        else ProgressionManager.Instance.ViewedDialogs.Add(dialogSequence.ID);

        if (gameplayDialog)
        {
            GameManager.Instance.GameplayDialog.LoadDialog(dialog, OnDialogFinished);
        }
        else
        {
            GameManager.Instance.DialogOverlay.LoadDialog(dialog, OnDialogFinished);
        }
    }
    #endregion

    #region Unity Stuff

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadDialog();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("meow");
    }
    #endregion
}
