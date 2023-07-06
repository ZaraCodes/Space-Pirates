using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
    #region Methods
    
    #endregion

    #region Unity Stuff
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
