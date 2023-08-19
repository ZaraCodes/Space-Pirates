using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Opens any kind of dialog
/// </summary>
public class TextboxTrigger : MonoBehaviour
{
    #region Fields
    /// <summary>The dialog sequence that will be loaded</summary>
    [SerializeField] private TextboxSequence dialogSequence;
    /// <summary>true if the dialog will be displayed as a gameplay dialog, false if it will be displayed in the dialog overlay</summary>
    [SerializeField] private bool gameplayDialog;
    
    /// <summary>This array contains all the required progression flags that have to be set for this dialog to activate</summary>
    [SerializeField] private EProgressionFlag[] requirements;
    /// <summary>This arrac contains all progression flags that will prevent this dialog to be activated</summary>
    [SerializeField] private EProgressionFlag[] antiRequirements;

    /// <summary>Unity Event that gets triggered when the dialog has finished</summary>
    [SerializeField] private UnityEvent OnDialogFinished;

    /// <summary>Unity Event that gets triggered when the dialog starts</summary>
    [SerializeField] private UnityEvent OnDialogStarted;
    #endregion

    #region Methods
    /// <summary>
    /// If all requirements are fulfilled and anti requirements are not set, the dialog will load
    /// </summary>
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
            if (dialogSequence.SummaryDialog != null)
            {
                dialog = dialogSequence.SummaryDialog;
            }
            else return;
        }
        else if (!dialogSequence.Repeat) ProgressionManager.Instance.ViewedDialogs.Add(dialogSequence.ID);

        OnDialogStarted.Invoke();
        if (gameplayDialog)
        {
            GameManager.Instance.GameplayDialog.LoadDialog(dialog, OnDialogFinished);
        }
        else
        {
            if (GameManager.Instance.Nova != null)
            {
                GameManager.Instance.Nova.Animator.SetFloat("velocityX", 0);
                GameManager.Instance.Nova.Animator.SetFloat("velocityY", 0);
            }
            GameManager.Instance.DialogOverlay.LoadDialog(dialog, OnDialogFinished);
        }
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// If there is a trigger and Nova enters it, it will load the dialog
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadDialog();
        }
    }
    #endregion
}
