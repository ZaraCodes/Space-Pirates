using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : ActivatableObject
{
    #region Fields

    [Header("Barrier")]
    [SerializeField] private SpriteRenderer spriteBounds;
    [SerializeField] private SpriteRenderer spriteClosed;
    [SerializeField] private SpriteRenderer spriteOpen;
    [Space]
    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private BoxCollider2D ballCollider;


    [SerializeField] private bool horizontal;
    #endregion

    #region Methods
    protected override void Toggle()
    {
        if (State || (invertState && !State))
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
    #endregion
    #region Unity Stuff

    private void Start()
    {
        Toggle();
    }

#if UNITY_EDITOR
    private void _OnValidate()
    {
        if (horizontal)
        {
            spriteBounds.sortingOrder = -Mathf.RoundToInt(spriteBounds.transform.position.y - 1);

            spriteClosed.size = spriteBounds.size;
            spriteClosed.sortingOrder = spriteBounds.sortingOrder;
            spriteOpen.size = new(spriteBounds.size.x, 1);
            spriteOpen.transform.localPosition = new Vector3(0, -(spriteBounds.size.y / 2f) + .5f, 0);
            spriteOpen.sortingOrder = spriteBounds.sortingOrder;

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
    #endregion
}
