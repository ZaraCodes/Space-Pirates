using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float movementSpeed;

    private Vector2 velocity;

    #region Unity Stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.gameObject.tag);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contactPoint = collision.GetContact(0);

        Vector2 newVelocity = Vector2.Reflect(velocity, contactPoint.normal);

        rb.velocity = newVelocity;
    }

    private void Start()
    {
        rb.velocity = Vector2.up * movementSpeed;
    }

    private void FixedUpdate()
    {
        velocity = rb.velocity;
    }


    #endregion
}
   