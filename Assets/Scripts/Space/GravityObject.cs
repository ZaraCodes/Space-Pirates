using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A gravity object in space can create a gravitational force for the hope ship depending on its mass and distance
/// </summary>
public class GravityObject : MonoBehaviour
{
    /// <summary>The mass of this gravity object</summary>
    [SerializeField] private float mass;
    /// <summary>The mass of this gravity object</summary>
    public float Mass { get { return mass; } }
    /// <summary>The scene to load when landing on this object</summary>
    [SerializeField] private int sceneIndexToLoad;
    /// <summary>The scene to load when landing on this object</summary>
    public int SceneIndexToLoad { get { return sceneIndexToLoad; } }
    /// <summary>The location associated with this gravity object</summary>
    [SerializeField] private ELastVisitedLocation location;
    /// <summary>The location associated with this gravity object</summary>
    public ELastVisitedLocation Location { get { return location; } }
    /// <summary>The distance at which the hope ship will orbit in</summary>
    [field: SerializeField] public float OrbitDistance { get; set; }
    /// <summary>The speed at which the orbit happens</summary>
    [field: SerializeField] public float OrbitSpeed { get; private set; }
}
