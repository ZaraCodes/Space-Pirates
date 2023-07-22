using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneMovementTrigger : MonoBehaviour
{
    #region Fields
    [SerializeField] private CutsceneMovement cutsceneMovement;
    /// <summary>This array contains all the required progression flags that have to be set for this dialog to activate</summary>
    [SerializeField] private EProgressionFlag[] requirements;
    /// <summary>This arrac contains all progression flags that will prevent this dialog to be activated</summary>
    [SerializeField] private EProgressionFlag[] antiRequirements;

    [SerializeField] private UnityEvent OnMovementStarted;
    #endregion

    #region Methods
    public void StartCutscene()
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
        cutsceneMovement.Activate();
        OnMovementStarted?.Invoke();
    }
    #endregion

    #region Unity Stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCutscene();
        }
    }
    #endregion

}
