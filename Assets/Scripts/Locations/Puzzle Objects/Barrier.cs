using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An activatable barrier that blocks Nova and Bullets from going through when it's inactive (closed)
/// </summary>
public class Barrier : ActivatableObject
{
    #region Fields
    /// <summary>Reference to the barrier's sprite renderer</summary>
    [Header("Barrier"), SerializeField] private SpriteRenderer spriteBounds;
    /// <summary>The barrier's closed sprite </summary>
    [SerializeField] private SpriteRenderer spriteClosed;
    /// <summary>The barrier's open sprite</summary>
    [SerializeField] private SpriteRenderer spriteOpen;
    /// <summary>The barrier's collider for Nova</summary>
    [Space, SerializeField] private BoxCollider2D playerCollider;
    /// <summary>The barrier's collider for bullets</summary>
    [SerializeField] private BoxCollider2D ballCollider;

    /// <summary>Bool that defines if the barrier has a horizontal orientation or a vertical one</summary>
    [SerializeField] private bool horizontal;
    #endregion

    #region Methods
    /// <summary>Toggle the barrier on or off</summary>
    protected override void Toggle()
    {
        if ((!invertState && State) || (invertState && !State))
        {
            spriteClosed.enabled = false;
            spriteOpen.enabled = true;
            ballCollider.enabled = false;
            playerCollider.enabled = false;
            if (!horizontal)
            {
                spriteClosed.sortingOrder = -9999;
            }
        }
        else
        {
            spriteClosed.enabled = true;
            spriteOpen.enabled = false;
            ballCollider.enabled = true;
            playerCollider.enabled = true;
            
            spriteOpen.sortingOrder = -9999;
            
        }
    }
    #endregion

    #region Unity Stuff

    private void Start()
    {
        Toggle();
        spriteClosed.sortingOrder = -Mathf.RoundToInt(spriteBounds.transform.position.y * 16) + 8;
    }

#if UNITY_EDITOR
    private void _OnValidate()
    {
        if (horizontal)
        {
            spriteBounds.sortingOrder = -Mathf.RoundToInt(spriteBounds.transform.position.y /*- 1*/- 1);

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
        else
        {
            spriteBounds.sortingOrder = -Mathf.RoundToInt(spriteBounds.transform.position.y - 1);

            spriteClosed.size = new(1, spriteBounds.size.y);
            spriteClosed.sortingOrder = spriteBounds.sortingOrder;
            spriteClosed.transform.localPosition = new(0, -.25f);
            spriteOpen.size = new(1, spriteBounds.size.y - 2.5f);
            spriteOpen.transform.localPosition = new Vector3(0, -1.5f);
            spriteOpen.sortingOrder = spriteBounds.sortingOrder;

            playerCollider.offset = new Vector2(0, -1.5f);
            playerCollider.size = new Vector2(0.25f, spriteBounds.size.y - 2.5f);
            ballCollider.offset = new Vector2(0, -.5f);
            ballCollider.size = new Vector2(0.25f, spriteBounds.size.y - 2.5f);
        }

        UnityEditor.EditorApplication.update -= _OnValidate;
    }

    private void OnValidate()
    {
        // UnityEditor.EditorApplication.update += _OnValidate;
    }
#endif
    #endregion
}
