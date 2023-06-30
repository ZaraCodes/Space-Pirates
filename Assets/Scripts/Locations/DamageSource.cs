using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [field: SerializeField] public DamageOriginator Origin { get; set; }
    [SerializeField] private int damage;
    [SerializeField] private float knockback;
}
