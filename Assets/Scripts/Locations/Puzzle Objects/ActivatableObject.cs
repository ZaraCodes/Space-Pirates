using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableObject : MonoBehaviour
{
    #region Fields
    [Header("Activatable Object")]
    [SerializeField] protected ToggleObject[] toggleObjects;
    [SerializeField] protected ActivationMode activationMode;
    [SerializeField] protected bool invertState;
    protected int activeInputs;
    public int ActiveInputs
    {
        get { return activeInputs; }
        set
        {
            activeInputs = value;
            if (activationMode == ActivationMode.And)
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

    protected bool state;
    public bool State
    {
        get { return state; }
        protected set
        {
            state = value;
            Toggle();
        }
    }
    #endregion

    #region Methods
    protected abstract void Toggle();

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
    }

    private void OnDisable()
    {
        foreach (var toggleObject in toggleObjects)
        {
            toggleObject.SwitchToggleEvent -= value => ReceiveInput(value);
        }
    }
    #endregion
}
