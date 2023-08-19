using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>A gravity receiver calculates the force that the gravity objects have on it</summary>
public class GravityReceiver : MonoBehaviour
{
    /// <summary>The Transform which contains all the gravity objects</summary>
    [SerializeField] private Transform gravityObjects;
    /// <summary>The gravitational constant (not realistic, but usable)</summary>
    private static float gravitationalConstant = 6.67430e-1f;

    /// <summary>
    /// Calculates the directional force at the position of the gravity receiver
    /// </summary>
    /// <returns>The directional force at the position of the gravity receiver</returns>
    public Vector2 CalculateForce()
    {
        return CalculateForce(transform.position);
    }

    /// <summary>
    /// Calculates the directional force.
    /// </summary>
    /// <param name="position">The position in space at which the force will be calculated</param>
    /// <returns>The directional force at the given position</returns>
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

    /// <summary>
    /// Calcualtes the force depending on the given mass of the gravity object used and the distance to it
    /// </summary>
    /// <param name="mass">The mass of the gravity object</param>
    /// <param name="distance">The distance to the gravity object</param>
    /// <returns>The strength of the directional force</returns>
    private static float CalculaceGravityForce(float mass, float distance)
    {
        return gravitationalConstant * (mass / Mathf.Pow(distance, 2f));
    }
}
