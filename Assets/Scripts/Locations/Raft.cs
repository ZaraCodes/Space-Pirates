using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raft : MonoBehaviour
{
    #region Fields
    [SerializeField] private bool move;
    [SerializeField] private float accelerationTime;
    private float accelerationTimer = 0;

    [Header("References"), SerializeField] private BoxCollider2D NorthCollision;
    [SerializeField] private BoxCollider2D SouthCollision;
    [SerializeField] private BoxCollider2D EastCollision;
    [SerializeField] private BoxCollider2D WestCollision;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private InteractableTrigger interactableTrigger;

    #endregion
    #region Methods
    private void EnableCollisions(bool enabled)
    {
        NorthCollision.enabled = enabled;
        SouthCollision.enabled = enabled;
        EastCollision.enabled = enabled;
        WestCollision.enabled = enabled;
    }

    public void SetMove(bool move) => this.move = move;

    #endregion
    #region Unity Stuff
    private void FixedUpdate()
    {
        if (accelerationTimer > 0)
        {
            accelerationTimer -= Time.fixedDeltaTime;
            rb.AddForce(new Vector3(.7f, 0), ForceMode2D.Force);
        }

        if (move)
        {
            EnableCollisions(true);
            move = false;
            accelerationTimer = accelerationTime;
            interactableTrigger.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform.parent.TryGetComponent(out NovaMovement player))
            {
                player.MovableObject = rb;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform.parent.TryGetComponent(out NovaMovement player))
            {
                player.MovableObject = null;
            }
        }
    }
    #endregion
}
