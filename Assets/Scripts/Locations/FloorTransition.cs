using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTransition : MonoBehaviour
{
    #region Fields
    [SerializeField] private bool groundFloor;
    [SerializeField] private bool playFallAnimation;
    [SerializeField] private float deactivationTime = 6f;
    private float deactivationTimer;

    bool triggerEnabled;
    #endregion
    #region Unity Stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggerEnabled) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playFallAnimation)
            {
                GameManager.Instance.Nova.BeginFall();
                deactivationTimer = deactivationTime;
                triggerEnabled = false;
            }
            else GameManager.Instance.Nova.SwitchFloor(groundFloor);
        }
    }

    private void Update()
    {
        if (!triggerEnabled)
        {
            deactivationTimer -= Time.deltaTime;
            if (deactivationTimer < 0)
            {
                triggerEnabled = true;
            }
        }
    }
    #endregion
}
