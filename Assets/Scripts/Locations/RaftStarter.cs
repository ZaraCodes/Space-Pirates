using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftStarter : ActivatableObject
{
    #region Fields
    [SerializeField] private Raft raft;
    [SerializeField] private BoxCollider2D entry;
    [SerializeField] private MovableBox movableBox;
    private Vector3 startPositionRaft;
    private Vector3 startPositionBox;
    #endregion

    #region Methods

    private void Start()
    {
        startPositionRaft = raft.transform.position;
        startPositionBox = movableBox.transform.position;
    }
    protected override void Toggle()
    {
        if (State)
        {
            raft.SetMove(true);
        }
        else
        {
            raft.transform.position = startPositionRaft;
            raft.ResetRaft();
            entry.enabled = false;
            if (movableBox != null)
            {
                movableBox.transform.position = startPositionBox;
                movableBox.BoxRigidbody.velocity = new();
                movableBox.MovableObject = null;
            }
        }
    }
    #endregion
}
