using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    [SerializeField] private float mass;

    [SerializeField] private int sceneIndexToLoad;

    [field: SerializeField] public float OrbitDistance { get; set; }

    public int SceneIndexToLoad { get { return sceneIndexToLoad; } }
    public float Mass { get { return mass; } }
}
