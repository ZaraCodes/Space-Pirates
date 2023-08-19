using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Toggle objects can be enabled or disabled and trigger an event when they do, but don't have any input unlike activatable objects
/// </summary>
public class ToggleObject : MonoBehaviour
{
    #region Fields
    /// <summary>Delegate that gets used for the switch toggle event</summary>
    /// <param name="state">The new state from this object</param>
    public delegate void SwitchToggle(bool state);
    /// <summary>Event that triggers when a ToggleObject switches its state</summary>
    public event SwitchToggle SwitchToggleEvent;

    /// <summary>The current state of this object</summary>
    [SerializeField] private bool state;
    /// <summary>Property that grants access to the private state variable and executes a state change when needed, which includes setting a sprite and invoking an event</summary>
    public bool State
    {
        get { return state; }
        protected set
        {
            state = value;
            if (state)
            {
                spriteRenderer.sprite = ActiveSprite;
                SwitchToggleEvent?.Invoke(state);
            }
            else
            {
                spriteRenderer.sprite = InactiveSprite;
                SwitchToggleEvent?.Invoke(state);
            }
            //Todo: Play sound
        }
    }

    /// <summary>Reference to the sprite renderer of this toggle object</summary>
    [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }
    /// <summary>The sprite that is used when the toggle object is active</summary>
    [SerializeField] private Sprite ActiveSprite;
    /// <summary>The sprite that is used when the toggle object is inactive</summary>
    [SerializeField] private Sprite InactiveSprite;

    /// <summary>Referenec to the audio clip that gets played when the toggle object gets enabled</summary>
    [Header("Audio Assets"), SerializeField] private AudioClip enableClip;
    /// <summary>Reference to the audio clip that gets played when the toggle object gets disabled</summary>
    [SerializeField] private AudioClip disableClip;
    #endregion

    #region Unity Stuff
    private void OnValidate()
    {
        State = State;
        SwitchToggleEvent?.Invoke(State);
    }
    #endregion
}
