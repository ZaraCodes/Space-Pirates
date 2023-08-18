using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activatable objects can influence and interact with each other. They can be used to create puzzle mechanics, like doors and barriers
/// </summary>
public abstract class ActivatableObject : MonoBehaviour
{
    #region Fields
    /// <summary>An array of toggle objects that can trigger this activatable object</summary>
    [Header("Activatable Object"), SerializeField] protected ToggleObject[] toggleObjects;
    /// <summary>All activatable objects that can trigger this activatable object</summary>
    [SerializeField] protected ActivatableObject[] activatableObjects;
    /// <summary>The activation mode of this activatable object. Either "or" or "and"</summary>
    [SerializeField] protected EActivationMode activationMode;
    /// <summary>Inverts the state of this activatable object</summary>
    [SerializeField] protected bool invertState;

    /// <summary>The current amount of active inputs. With activation mode "or", it has to be greater than 0, and with activation mode "and" it has to match the number of connected objects</summary>
    protected int activeInputs;
    /// <summary>The current amount of active inputs. With activation mode "or", it has to be greater than 0, and with activation mode "and" it has to match the number of connected objects</summary>
    public int ActiveInputs
    {
        get { return activeInputs; }
        private set
        {
            activeInputs = value;
            if (activationMode == EActivationMode.And)
            {
                if (activeInputs == toggleObjects.Length) State = true;
                else State = false;
            }
            else
            {
                if (activeInputs > 0) State = true;
                else State = false;
            }
        }
    }

    /// <summary>The current state of this activatable object</summary>
    protected bool state;
    /// <summary>If a value gets set that is not the current value, the OnStageChanged event gets invoked, which interacts with other interactable objects that this object is connected to</summary>
    public bool State
    {
        get { return state; }
        protected set
        {
            if (state != value)
            {
                OnStateChanged?.Invoke(value);
            }
            state = value;
            Toggle();
        }
    }

    /// <summary>Delegate that is used for the OnStateChanged event</summary>
    /// <param name="state">The new state that will be sent to the objects this activatable object is connected to</param>
    public delegate void ChangeState(bool state);
    /// <summary>An event that toggles when the state of this interactable object changes</summary>
    public event ChangeState OnStateChanged;
    #endregion

    #region Methods
    /// <summary>Template Method that gets implemented by child objects of ActivatableObject. It gets called when the State property gets set</summary>
    protected abstract void Toggle();

    /// <summary>This method is applied to the State Change event of the toggle objects and activatable objects that are connected to this activatable object.</summary>
    /// <param name="input">If true, Active Inputs get increased, else decreased</param>
    protected void ReceiveInput(bool input)
    {
        if (input) ActiveInputs++;
        else ActiveInputs--;
    }
    #endregion

    #region Unity Stuff

    private void OnEnable()
    {
        foreach (var toggleObject in toggleObjects)
        {
            toggleObject.SwitchToggleEvent += value => ReceiveInput(value);
        }
        foreach (var activatableObject in activatableObjects)
        {
            activatableObject.OnStateChanged += value => ReceiveInput(value);
        }
    }

    private void OnDisable()
    {
        foreach (var toggleObject in toggleObjects)
        {
            toggleObject.SwitchToggleEvent -= value => ReceiveInput(value);
        }
        foreach (var activatableObject in activatableObjects)
        {
            activatableObject.OnStateChanged -= value => ReceiveInput(value);
        }
    }
    #endregion
}
