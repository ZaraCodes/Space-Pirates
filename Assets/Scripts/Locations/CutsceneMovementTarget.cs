using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene Movement Target", menuName = "Cutscene/Movement Target")]
public class CutsceneMovementTarget : ScriptableObject
{
    public Vector3 Target;
    public PrioritizedAxis PrioritizedAxis;
}

public enum PrioritizedAxis
{
    X, Y
}
