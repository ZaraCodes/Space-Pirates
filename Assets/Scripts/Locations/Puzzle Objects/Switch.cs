using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A switch is a toggle object that switches its state if it gets hit by a bullet
/// </summary>
public class Switch : ToggleObject
{
    #region Fields
    /// <summary>Activation time of the switch. If it is greater than 0, in case of a state change it will stay active for the specified amount of time and then deactivate itself</summary>
    [Header("Switch Attributes"), SerializeField] private float activationTime = -1;

    /// <summary>If true, the switch will stay active and will not be interactable anymore once it gets activated</summary>
    [SerializeField] private bool disableOnActivate;

    /// <summary>Timer that handles the activation time if it is set</summary>
    private float timer;
    #endregion

    #region Methods
    /// <summary>Toggles the state</summary>
    public void Toggle()
    {
        State = !State;
        if (activationTime > 0f)
        {
            if (State) timer = activationTime;
            else timer = 0f;
        }
    }

    /// <summary>Sets the state</summary>
    /// <param name="state">the new state value</param>
    public void SetState(bool state)
    {
        State = state;
    }
    #endregion

    #region Unity Stuff
    /// <summary>When a bullet gets enters this trigger, it can toggle the switch if the conditions are met</summary>
    /// <param name="collision">The collider of the other object</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(State && disableOnActivate))
        {
            Toggle();

            if (collision.gameObject.CompareTag("Bullet"))
            {
                Destroy(collision.gameObject);
            }
        }
    }
    /// <summary>
    /// if the activation time is bigger than 0 it will do the countdown
    /// </summary>
    private void Update()
    {
        if (activationTime > 0f && timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                State = !State;
            }
        }
    }
    #endregion
}
