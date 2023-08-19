using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This object destroys bullets that enter its trigger
/// </summary>
public class BulletDestroyer : MonoBehaviour
{
    #region Unity Stuff
    /// <summary>
    /// Destroys a bullet if it enters this trigger
    /// </summary>
    /// <param name="collision">Collider of the bullet that enters the trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.TryGetComponent(out ChargedBullet chargedBullet)) chargedBullet.PlayDestroySound = false;
            Destroy(collision.gameObject);
        }
    }
    #endregion
}
