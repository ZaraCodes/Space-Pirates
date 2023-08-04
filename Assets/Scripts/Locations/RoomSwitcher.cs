using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSwitcher : MonoBehaviour
{
    #region Fields
    [SerializeField] private CinemachineConfiner2D camConfiner;
    [SerializeField] private bool oversizeWindow;
    #endregion

    public void SetConfiner(Collider2D col)
    {
        camConfiner.m_BoundingShape2D = col;
        camConfiner.m_MaxWindowSize = 568;
        //camConfiner.m_Damping = 0f;
    }
}
