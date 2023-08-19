using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scales the canvas to the UI scale set in the settings
/// </summary>
public class CanvasAutoScaler : MonoBehaviour
{
    /// <summary>
    /// Reference to the canvas scaler
    /// </summary>
    [SerializeField] CanvasScaler scaler;

    /// <summary>
    /// Sets the UI scale when this object gets enabled
    /// </summary>
    private void OnEnable()
    {
        scaler.scaleFactor = SettingsS.Instance.UIScale;
    }
}
