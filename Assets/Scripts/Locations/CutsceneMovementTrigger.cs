using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Trigger that starts a cutscene movement segment where Nova gets moved automatically and the player doesn't have control
/// </summary>
public class CutsceneMovementTrigger : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the cutscene movement object</summary>
    [SerializeField] private CutsceneMovement cutsceneMovement;
    /// <summary>This array contains all the required progression flags that have to be set for this dialog to activate</summary>
    [SerializeField] private EProgressionFlag[] requirements;
    /// <summary>This arrac contains all progression flags that will prevent this dialog to be activated</summary>
    [SerializeField] private EProgressionFlag[] antiRequirements;
    /// <summary>Unity event that gets triggered when the movement starts</summary>
    [SerializeField] private UnityEvent OnMovementStarted;
    #endregion

    #region Methods
    /// <summary>
    /// Starts the cutscene movement and invokes the movement started event if the requirements are met and the anti requirements are not set
    /// </summary>
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
    /// <summary>
    /// If the player enters the trigger, the cutscene movement will start
    /// </summary>
    /// <param name="collision">Collision of the other object</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCutscene();
        }
    }
    #endregion

}
