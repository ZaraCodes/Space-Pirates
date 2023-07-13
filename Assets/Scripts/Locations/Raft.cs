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
    [SerializeField] private bool movementAxisY;
    private float accelerationTimer = 0;
    private Vector3 movementDirectionSnapshot;

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

    private void StopMovement()
    {
        rb.bodyType = RigidbodyType2D.Static;
        if (movementAxisY)
        {
            if (movementDirectionSnapshot.y < 0) NorthCollision.enabled = false;
            else SouthCollision.enabled = false;
        }
        else
        {
            if (movementDirectionSnapshot.x < 0) WestCollision.enabled = false;
            else EastCollision.enabled = false; 
        }
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
            rb.bodyType = RigidbodyType2D.Dynamic;
            EnableCollisions(true);
            move = false;
            accelerationTimer = accelerationTime;
            interactableTrigger.gameObject.SetActive(false);
        }
        if (brake)
        {
            var decelerationVector = new Vector3(-3, 0);
            move = false;
            accelerationTimer = 0;
            if (movementAxisY && ((movementDirectionSnapshot.y > 0 && rb.velocity.y < 0) || (movementDirectionSnapshot.y < 0 && rb.velocity.y > 0)))
                StopMovement();
            else if (!movementAxisY && ((movementDirectionSnapshot.x > 0 && rb.velocity.x < 0) || (movementDirectionSnapshot.x < 0 && rb.velocity.x > 0)))
                StopMovement();
            rb.AddForce(decelerationVector);
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
            movementDirectionSnapshot = rb.velocity;
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
    }

    private void Start()
    {
        if (movementAxisY) rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        else rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }
    #endregion
}
