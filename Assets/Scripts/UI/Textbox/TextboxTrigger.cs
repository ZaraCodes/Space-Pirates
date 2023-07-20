using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// [RequireComponent(typeof(BoxCollider2D))]
public class TextboxTrigger : MonoBehaviour
{
    #region Fields
    [SerializeField] private string DialogID;
    [SerializeField] private bool gameplayDialog;
    
    /// <summary>This array contains all the required progression flags that have to be set for this dialog to activate</summary>
    [SerializeField] private EProgressionFlag[] requirements;

    [SerializeField] private UnityEvent OnDialogFinished;

    #endregion

    #region Methods
    public void LoadDialog()
    {
        foreach (var requirement in requirements)
        {
            if (!ProgressionManager.Instance.Flags.Contains(requirement))
            {
                return;
            }
        }
        TextboxSequence dialogSequence = AllDialogs.Instance.GetSequence(DialogID);

        if (dialogSequence == null)
        {
            Debug.LogWarning($"The dialog \"{DialogID}\" does not exist!");
            return;
        }
        if (ProgressionManager.Instance.ViewedDialogs.Contains(DialogID))
        {
            if (!dialogSequence.Repeat) return;
        }
        else ProgressionManager.Instance.ViewedDialogs.Add(DialogID);

        if (gameplayDialog)
        {
            //Todo: Test for progression / story flag

            GameManager.Instance.GameplayDialog.LoadDialog(dialogSequence, OnDialogFinished);
        }
        else
        {
            //Todo: Test for progression / story flag

            GameManager.Instance.DialogOverlay.LoadDialog(dialogSequence, OnDialogFinished);
            //overlay.LoadDialog(DialogID);
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
