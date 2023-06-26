using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityReceiver : MonoBehaviour
{
    [SerializeField] private Transform gravityObjects;

    private static float gravitationalConstant = 6.67430e-1f;

    public Vector2 CalculateForce()
    {
        return CalculateForce(transform.position);
    }

    public Vector2 CalculateForce(Vector3 position)
    {
        Vector2 force = Vector2.zero;
        if (gravityObjects != null)
        {
            foreach (Transform t in gravityObjects)
            {
                var gravityObject = t.GetComponent<GravityObject>();
                if (gravityObject != null)
                {
                    var distance = (t.position - position);
                    float objectForce = CalculaceGravityForce(gravityObject.Mass, distance.magnitude);
                    force += new Vector2(distance.normalized.x * objectForce, distance.normalized.y * objectForce);
                }
            }
        }
        return force;
    }

    private static float CalculaceGravityForce(float mass, float distance)
    {
        return gravitationalConstant * (mass / Mathf.Pow(distance, 2f));
    }
}
