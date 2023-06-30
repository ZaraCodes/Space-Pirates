using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedBullet : MonoBehaviour
{
    /// <summary>Reference to the ball's rigidbody</summary>
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb { get { return rb; } }

    /// <summary>The speed at which the ball will move</summary>
    [SerializeField] private float movementSpeed;
    public float MovementSpeed { get { return movementSpeed; } }

    /// <summary>The maximum amount of bounces the ball will do before destroying itself.</summary>
    [SerializeField] private int maxBounces;
    
    /// <summary>The amount of bounces the ball performed.</summary>
    private int bounces;

    /// <summary>Velocity of the ball</summary>
    private Vector2 velocity;

    #region Unity Stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.gameObject.tag);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (++bounces >= maxBounces)
        {
            Destroy(gameObject);
        }


        ContactPoint2D contactPoint = collision.GetContact(0);

        Vector2 newVelocity = Vector2.Reflect(velocity, contactPoint.normal);

        rb.velocity = newVelocity;

    }

    private void Start()
    {
        // rb.velocity = new Vector2(1,1).normalized * movementSpeed;
        bounces = 0;
    }

    private void FixedUpdate()
    {
        velocity = rb.velocity;
    }

    private void OnDestroy()
    {
        //Todo: Play ball destroyed animation.
    }


    #endregion
}
   