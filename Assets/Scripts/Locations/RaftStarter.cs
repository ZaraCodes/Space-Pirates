using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An activatable object that can make a raft move
/// </summary>
public class RaftStarter : ActivatableObject
{
    #region Fields
    /// <summary>Reference to the raft that should be moved</summary>
    [SerializeField] private Raft raft;
    /// <summary>Reference to the collider that should be disabled in case the raft gets reset</summary>
    [SerializeField] private BoxCollider2D entry;
    /// <summary>Reference to the movable box that may be standing on top of the raft</summary>
    [SerializeField] private MovableBox movableBox;
    /// <summary>If true the player will be reset when the reset command is given</summary>
    private bool doPlayerReset;
    /// <summary>The original position of the raft</summary>
    private Vector3 startPositionRaft;
    /// <summary>The original position of the box</summary>
    private Vector3 startPositionBox;
    #endregion

    #region Methods
    /// <summary>
    /// Sets if the player should be reset
    /// </summary>
    /// <param name="value">the bool value for resetting the player position</param>
    public void SetPlayerReset(bool value)
    {
        doPlayerReset = value;
    }

    /// <summary>
    /// Toggles the Raft Starter. If the state is true, then the raft starts moving. if it is false, the raft, box and player are reset
    /// </summary>
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
                if (doPlayerReset && !GameManager.Instance.Nova.IsOnGround()) GameManager.Instance.Nova.Respawn();
            }
        }
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// Sets the start positions and parameters
    /// </summary>
    private void Start()
    {
        startPositionRaft = raft.transform.position;
        startPositionBox = movableBox.transform.position;
        doPlayerReset = true;
    }

    #endregion
}
