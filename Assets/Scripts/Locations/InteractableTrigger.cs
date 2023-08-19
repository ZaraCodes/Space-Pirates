using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

/// <summary>
/// An InteractableTrigger spawns an interaction prompt if Nova enters it, and executes that action if the player hits the specified button
/// </summary>
public class InteractableTrigger : MonoBehaviour
{
    #region Fields
    /// <summary>If true, the trigger will show the interaction prompt</summary>
    [SerializeField] private bool showText = true;
    /// <summary>Reference to the localized string that gets displayed in the interaction prompt</summary>
    [SerializeField] private LocalizedString interactText;
    /// <summary>Property that allows read access to the localized string</summary>
    public LocalizedString InteractText { get { return interactText; } }

    /// <summary>The interaction mode for the interaction trigger</summary>
    [SerializeField] private EInteractMode interactMode;

    /// <summary>Unity Event that gets triggered for the interaction. Only once if the mode is press, repeatedly if the mode is hold</summary>
    public UnityEvent OnInteract;
    /// <summary>Unity Event that gets triggered when the interaction stops</summary>
    public UnityEvent OnInteractStop;

    /// <summary>true if it gets interacted with</summary>
    public bool interact;
    #endregion

    #region Methods
    /// <summary>
    /// This method gets executed when the interaction starts and performs the interaction depending on the interaction mode
    /// </summary>
    public void Interact()
    {
        if (interactMode == EInteractMode.Press)
        {
            OnInteract.Invoke();
        }
        else
        {
            interact = true;
            StartCoroutine(RepeatInteract());
        }
    }

    /// <summary>
    /// Stops an interaction and invokes the stop event
    /// </summary>
    public void StopInteract()
    {
        OnInteractStop.Invoke();
        interact = false;
    }

    /// <summary>Coroutine that repeats an interaction each frame</summary>
    /// <returns></returns>
    public IEnumerator RepeatInteract()
    {
        while (interact)
        {
            OnInteract.Invoke();
            yield return null;
        }
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// Shows the Interaction prompt if the conditions are met
    /// </summary>
    /// <param name="collision">Collision of the other object</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.name == "Button Trigger Nova" && showText && (GameManager.Instance.IsPlaying || ProgressionManager.Instance.Flags.Contains(EProgressionFlag.IntroFinished)))
        if (collision.name == "Button Trigger Nova" && showText && !(!GameManager.Instance.IsPlaying && !ProgressionManager.Instance.Flags.Contains(EProgressionFlag.IntroFinished)))
        {
            GameManager.Instance.Nova.ShowInteractionPrompt(this);
        }
    }
    #endregion
}
