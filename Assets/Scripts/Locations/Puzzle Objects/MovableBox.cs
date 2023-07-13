using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBox : MonoBehaviour
{
    [SerializeField] private InteractableTrigger horizontalTrigger;
    [SerializeField] private InteractableTrigger verticalTrigger;
    [HideInInspector] public Rigidbody2D MovableObject;

    private Rigidbody2D novaRigidbody;
    [SerializeField] private Rigidbody2D boxRigidbody;
    [SerializeField] private SpriteRenderer boxSprite;

    private Vector2 moveVector;

    #region Methods
    public void Move(bool horizontal)
    {
        if (novaRigidbody != null || GameObject.Find("Nova").TryGetComponent(out novaRigidbody))
        {
            boxRigidbody.bodyType = RigidbodyType2D.Dynamic;
            if (horizontal)
            {
                moveVector = new Vector2(novaRigidbody.velocity.x, 0);
            }
            else
            {
                moveVector = new Vector2(0, novaRigidbody.velocity.y);
            }
            // while interacting with the box, nova should only move along the same axis as the box
            // novaRigidbody.velocity = boxRigidbody.velocity;
        }
    }

    public void StopMoving()
    {
        if (boxRigidbody.velocity != Vector2.zero)
        {
            moveVector = Vector2.zero;
            //boxRigidbody.bodyType = RigidbodyType2D.Static;
        }
    }
    #endregion

    #region Unity Stuff
    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            boxSprite.sortingOrder = -Mathf.RoundToInt(transform.position.y);
        }
    }

    private void FixedUpdate()
    {
        boxRigidbody.velocity = moveVector;
        if (MovableObject != null) boxRigidbody.velocity += MovableObject.velocity;
    }
    #endregion

}
