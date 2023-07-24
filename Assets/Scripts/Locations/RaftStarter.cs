using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftStarter : ActivatableObject
{
    #region Fields
    [SerializeField] private Raft raft;
    #endregion

    #region Methods
    protected override void Toggle()
    {
        if (State)
        {
            raft.SetMove(true);
        }
    }
    #endregion
}
