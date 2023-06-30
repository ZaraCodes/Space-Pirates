using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    #region Fields
    [SerializeField] private ToggleObject toggleObject;

    [SerializeField] private SpriteRenderer spriteBounds;
    [SerializeField] private SpriteRenderer spriteClosed;
    [SerializeField] private SpriteRenderer spriteOpen;
    [Space]
    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private BoxCollider2D ballCollider;


    [SerializeField] private bool horizontal;

    private bool state;
    public bool State
    {
        get { return state; }
        private set
        {
            state = value;
            ToggleBarrier();
        }
    }

    #endregion

    private void ToggleBarrier()
    {
        if (State)
        {
            spriteClosed.enabled = false;
            spriteOpen.enabled = true;
            ballCollider.enabled = false;
            playerCollider.enabled = false;
        }
        else
        {
            spriteClosed.enabled = true;
            spriteOpen.enabled = false;
            ballCollider.enabled = true;
            playerCollider.enabled = true;
        }
    }


    #region Unity Stuff

#if UNITY_EDITOR
    private void _OnValidate()
    {
        if (horizontal)
        {
            spriteClosed.size = spriteBounds.size;
            spriteOpen.size = new(spriteBounds.size.x, 1);
            spriteOpen.transform.localPosition = new Vector3(0, -(spriteBounds.size.y / 2f) + .5f, 0);
            playerCollider.offset = new Vector2(0, -(spriteBounds.size.y / 2f) + .5f);
            playerCollider.size = new Vector2(spriteBounds.size.x, 0.25f);
            ballCollider.offset = new Vector2(0, -(spriteBounds.size.y / 2f) + 1.5f);
            ballCollider.size = new Vector2(spriteBounds.size.x, 0.25f);
        }

        UnityEditor.EditorApplication.update -= _OnValidate;
    }

    private void OnValidate()
    {
        UnityEditor.EditorApplication.update += _OnValidate;
    }
#endif

    private void OnEnable()
    {
        toggleObject.SwitchToggleEvent += value => State = value;
    }

    private void OnDisable()
    {
        toggleObject.SwitchToggleEvent -= value => State = value;
    }
    #endregion
}
