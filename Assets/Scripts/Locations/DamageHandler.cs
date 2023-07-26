using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private DamageOriginator self;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<DamageSource>(out var damageSource))
        {
            if (damageSource.Origin != self)
            {
                Debug.Log("Oh nyo, I got hit!");
                
                //Todo: handle Damage
            }
        }
    }
}
