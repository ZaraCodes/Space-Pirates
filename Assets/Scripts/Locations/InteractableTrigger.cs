using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class InteractableTrigger : MonoBehaviour
{
    #region Fields
    [SerializeField] private bool showText = true;
    [SerializeField] private LocalizedString interactText;
    public LocalizedString InteractText { get { return interactText; } }

    [SerializeField] private EInteractMode interactMode;

    public UnityEvent OnInteract;
    public UnityEvent OnInteractStop;

    public bool interact;
    #endregion

    #region Methods
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

    public void StopInteract()
    {
        OnInteractStop.Invoke();
        interact = false;
    }

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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
        if (collision.name == "Button Trigger Nova" && showText && !(!GameManager.Instance.IsPlaying && !ProgressionManager.Instance.Flags.Contains(EProgressionFlag.IntroFinished)))
        {
            GameManager.Instance.Nova.ShowInteractionPrompt(this);
        }
    }
    #endregion
}
