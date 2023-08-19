using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This gets used in the space station to change the camera boundaries for the cinemachine camera
/// </summary>
public class RoomSwitcher : MonoBehaviour
{
    #region Fields
    /// <summary>Confiner of the current room</summary>
    [SerializeField] private CinemachineConfiner2D camConfiner;
    /// <summary>Oversize window?</summary>
    [SerializeField] private bool oversizeWindow;
    #endregion

    /// <summary>
    /// sets the confiner to the new collider
    /// </summary>
    /// <param name="col">The collider of the new room</param>
    public void SetConfiner(Collider2D col)
    {
        camConfiner.m_BoundingShape2D = col;
        camConfiner.m_MaxWindowSize = 568;
    }
}
