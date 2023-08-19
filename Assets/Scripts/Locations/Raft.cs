using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A raft is basically a movable platform that can move Nova and Movable Boxes
/// </summary>
public class Raft : MonoBehaviour
{
    #region Fields
    /// <summary>should it start moving</summary>
    [SerializeField] private bool move;
    /// <summary>should it start braking</summary>
    [SerializeField] private bool brake;
    /// <summary>The time it takes for the raft to accelerate to max speed</summary>
    [SerializeField] private float accelerationTime;
    /// <summary>The direction the raft will move in</summary>
    [SerializeField] private Vector2 movementDirection;
    /// <summary>The timer used for acceleration and deceleration</summary>
    private float accelerationTimer = 0;

    /// <summary>Reference to the north collision wall</summary>
    [Header("References"), SerializeField] private BoxCollider2D NorthCollision;
    /// <summary>Reference to the south collision wall</summary>
    [SerializeField] private BoxCollider2D SouthCollision;
    /// <summary>Reference to the east collision wall</summary>
    [SerializeField] private BoxCollider2D EastCollision;
    /// <summary>Reference to the west collision wall</summary>
    [SerializeField] private BoxCollider2D WestCollision;
    /// <summary>Reference to the raft's rigidbody</summary>
    [SerializeField] private Rigidbody2D rb;
    /// <summary>Interaction trigger for the raft</summary>
    [SerializeField] private InteractableTrigger interactableTrigger;
    /// <summary>An Array of colliders that will be disabled once the raft stops</summary>
    [SerializeField] private BoxCollider2D[] barriersToDisableOnArrival;
    #endregion

    #region Methods
    /// <summary>
    /// sets the enabled value for all collisions from the raft
    /// </summary>
    /// <param name="enabled">boolean that decides if the collisions are enabled or disabled</param>
    private void EnableCollisions(bool enabled)
    {
        NorthCollision.enabled = enabled;
        SouthCollision.enabled = enabled;
        EastCollision.enabled = enabled;
        WestCollision.enabled = enabled;
    }

    /// <summary>
    /// Once the raft has stopped, it changes to a static rigidbody and disables the barriers that should be disabled
    /// </summary>
    private void StopMovement()
    {
        brake = false;
        rb.velocity = Vector2.zero;
        foreach (var collider in barriersToDisableOnArrival)
        {
            collider.enabled = false;
        }
        rb.bodyType = RigidbodyType2D.Static;
    }

    /// <summary>
    /// Sets the move variable
    /// </summary>
    /// <param name="move">the new move value</param>
    public void SetMove(bool move) => this.move = move;

    /// <summary>
    /// Resets the raft to its original state
    /// </summary>
    public void ResetRaft()
    {
        accelerationTimer = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;
        move = false;
        brake = false;
        StopMovement();
        interactableTrigger.gameObject.SetActive(true);
    }

    #endregion

    #region Unity Stuff
    /// <summary>
    /// Handles the acceleration and braking of the raft
    /// </summary>
    private void FixedUpdate()
    {
        if (accelerationTimer > 0)
        {
            accelerationTimer -= Time.fixedDeltaTime;
            rb.velocity += new Vector2(movementDirection.x, movementDirection.y) * 0.02f;
        }

        if (move)
        {
            //rb.bodyType = RigidbodyType2D.Kinematic;
            EnableCollisions(true);
            interactableTrigger.gameObject.SetActive(false);
            move = false;
            accelerationTimer = accelerationTime;
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
            if (rb.bodyType != RigidbodyType2D.Static) rb.velocity += decelerationVector;
        }
    }

    /// <summary>
    /// If certain objects enter the raft, they can have certain effects. If Nova or a movable box enters a raft, they receive a reference to the raft so they can
    /// account for the raft's velociy. If a raft brake enters the trigger, it decelerates the raft
    /// </summary>
    /// <param name="collision">The collider of the other object</param>
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
            if (collision.transform.parent.TryGetComponent(out MovableBox box))
            {
                box.MovableObject = rb;
            }
        }
    }

    /// <summary>
    /// If the player or a box leaves the trigger, the reference to the raft gets destroyed
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.transform.parent.TryGetComponent(out NovaMovement player))
            {
                player.MovableObject = null;
            }
        }
        else if (collision.CompareTag("Button Trigger") && !move)
        {
            if (collision.transform.parent.TryGetComponent(out MovableBox box))
            {
                box.MovableObject = null;
            }
        }
    }

    /// <summary>
    /// Sets up the rb constraints for the raft
    /// </summary>
    private void Start()
    {
        if (movementDirection.y != 0) rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        else rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }
    #endregion
}
