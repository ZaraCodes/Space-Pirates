using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    #region Fields
    public delegate void SwitchToggle(bool state);
    public event SwitchToggle SwitchToggleEvent;

    [SerializeField] private bool state;

    public bool State
    {
        get { return state; }
        protected set
        {
            state = value;
            if (state)
            {
                spriteRenderer.color = ActiveColor;
                spriteRenderer.sprite = ActiveSprite;
                SwitchToggleEvent?.Invoke(state);
            }
            else
            {
                spriteRenderer.color = InactiveColor;
                spriteRenderer.sprite = InactiveSprite;
                SwitchToggleEvent?.Invoke(state);
            }
            //Todo: Play sound
        }
    }

    [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }
    [field: SerializeField] public Color ActiveColor { get; private set; }
    [field: SerializeField] public Color InactiveColor { get; private set; }

    [SerializeField] private Sprite ActiveSprite;
    [SerializeField] private Sprite InactiveSprite;

    [Header("Audio Assets")]
    [SerializeField] private AudioClip enableClip;
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
