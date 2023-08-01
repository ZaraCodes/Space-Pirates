using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Raft : MonoBehaviour
{
    #region Fields
    [SerializeField] private bool move;
    [SerializeField] private bool brake;
    [SerializeField] private float accelerationTime;
    [SerializeField] private Vector2 movementDirection;
    private float accelerationTimer = 0;

    [Header("References"), SerializeField] private BoxCollider2D NorthCollision;
    [SerializeField] private BoxCollider2D SouthCollision;
    [SerializeField] private BoxCollider2D EastCollision;
    [SerializeField] private BoxCollider2D WestCollision;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private InteractableTrigger interactableTrigger;
    [SerializeField] private BoxCollider2D[] barriersToDisableOnArrival;

    #endregion
    #region Methods
    private void EnableCollisions(bool enabled)
    {
        NorthCollision.enabled = enabled;
        SouthCollision.enabled = enabled;
        EastCollision.enabled = enabled;
        WestCollision.enabled = enabled;
    }

    private void StopMovement()
    {
        rb.bodyType = RigidbodyType2D.Static;
        brake = false;
        foreach (var collider in barriersToDisableOnArrival)
        {
            collider.enabled = false;
        }
    }

    public void SetMove(bool move) => this.move = move;

    public void ResetRaft()
    {
        accelerationTimer = 0;
        move = false;
        brake = false;
        StopMovement();
        interactableTrigger.gameObject.SetActive(true);
    }

    #endregion
    #region Unity Stuff
    private void FixedUpdate()
    {
        if (accelerationTimer > 0)
        {
            accelerationTimer -= Time.fixedDeltaTime;
            rb.velocity += new Vector2(movementDirection.x, movementDirection.y) * 0.02f;
            //rb.AddForce(new Vector3(movementDirection.x, movementDirection.y), ForceMode2D.Impulse);
        }

        if (move)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            EnableCollisions(true);
            move = false;
            accelerationTimer = accelerationTime;
            interactableTrigger.gameObject.SetActive(false);
        }
        if (brake)
        {
            var decelerationVector = -movementDirection * .15f;
;
            move = false;
            accelerationTimer = 0;
            if (movementDirection == Vector2.left && rb.velocity.x > 0 || movementDirection == Vector2.right && rb.velocity.x < 0)
                StopMovement();
            else if (movementDirection == Vector2.down && rb.velocity.y > 0 || movementDirection == Vector2.up && rb.velocity.y < 0)
                StopMovement();
            rb.velocity += decelerationVector;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.transform.parent.TryGetComponent(out NovaMovement player))
            {
                player.MovableObject = rb;
            }
        }
        else if (collision.CompareTag("Raft Brake"))
        {
            brake = true;
        }
        else if (collision.CompareTag("Button Trigger"))
        {
            var box = collision.GetComponentInParent<MovableBox>();
            if (box != null)
            {
                box.MovableObject = rb;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.transform.parent.TryGetComponent(out NovaMovement player))
            {
                player.MovableObject = null;
            }
        }
        //else if (collision.CompareTag("Button Trigger"))
        //{
        //    var box = collision.GetComponentInParent<MovableBox>();
        //    if (box != null)
        //    {
        //        box.MovableObject = null;
        //    }
        //}
    }

    private void Start()
    {
        if (movementDirection.y != 0) rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        else rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }
    #endregion
}
