using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    [SerializeField] private float mass;

    [SerializeField] private int sceneIndexToLoad;

    [SerializeField] private ELastVisitedLocation location;
    public ELastVisitedLocation Location { get { return location; } }

    [field: SerializeField] public float OrbitDistance { get; set; }

    [field: SerializeField] public float OrbitSpeed { get; private set; }

    public int SceneIndexToLoad { get { return sceneIndexToLoad; } }
    public float Mass { get { return mass; } }
}
