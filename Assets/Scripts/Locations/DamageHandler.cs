using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private DamageOriginator self;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageSource = collision.gameObject.GetComponent<DamageSource>();
        if (damageSource != null)
        {
            if (damageSource.Origin != self)
            {
                Debug.Log("Oh nyo, I got hit!");
                
                //Todo: handle Damage
            }
        }
    }
}
