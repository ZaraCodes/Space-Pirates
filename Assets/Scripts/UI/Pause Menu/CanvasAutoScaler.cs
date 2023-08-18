using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAutoScaler : MonoBehaviour
{
    [SerializeField] CanvasScaler scaler;
    private void OnEnable()
    {
        scaler.scaleFactor = SettingsS.Instance.UIScale;
    }
}
