using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptabe Object that contains data for the movement target
/// </summary>
[CreateAssetMenu(fileName = "New Cutscene Movement Target", menuName = "Cutscene/Movement Target")]
public class CutsceneMovementTarget : ScriptableObject
{
    /// <summary>
    /// The target position
    /// </summary>
    public Vector3 Target;

    /// <summary>
    /// The axis that Nova will move in first
    /// </summary>
    public EPrioritizedAxis PrioritizedAxis;
}

/// <summary>
/// Enum that contains values for axises that should be prioritized by the cutscene movement code
/// </summary>
public enum EPrioritizedAxis
{
    X, Y
}
