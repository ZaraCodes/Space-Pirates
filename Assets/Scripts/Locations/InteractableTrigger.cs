using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class InteractableTrigger : MonoBehaviour
{
    [SerializeField] private LocalizedString interactText;

    [SerializeField] private EInteractMode interactMode;

    public UnityEvent OnInteract;
    public UnityEvent OnInteractStop;

    public bool interact;

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
}
