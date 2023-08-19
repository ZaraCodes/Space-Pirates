using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Object that sets a progression flag
/// </summary>
public class ProgressionSetter : MonoBehaviour
{
    /// <summary>
    /// The progression flag that will be set
    /// </summary>
    [SerializeField] private EProgressionFlag flag;

    /// <summary>
    /// Sets the flag to the progression manager
    /// </summary>
    public void SetFlag()
    {
        if (!ProgressionManager.Instance.Flags.Contains(flag))
            ProgressionManager.Instance.Flags.Add(flag);
    }
}
