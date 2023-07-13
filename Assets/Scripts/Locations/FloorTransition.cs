using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTransition : MonoBehaviour
{
    #region Fields
    [SerializeField] private TransitionDirection direction;
    [SerializeField] private bool groundFloor;
    [SerializeField] private bool playFallAnimation;

    public bool TriggerEnabled;
    #endregion
    #region Unity Stuff
    private void Start()
    {
        TriggerEnabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playFallAnimation)
            {
                if (GameManager.Instance.Nova.DoFall && GameManager.Instance.Nova.FallTimer > GameManager.Instance.Nova.FallTime / 4f)
                {
                    if (GameManager.Instance.Nova.TransitionTrigger == null || (GameManager.Instance.Nova.TransitionTrigger != null && GameManager.Instance.Nova.TransitionTrigger != this))
                        GameManager.Instance.Nova.StopFall(true);
                }
                else
                {
                    if (!TriggerEnabled) return;

                    if (GameManager.Instance.Nova.TransitionTrigger != null)
                    {
                        GameManager.Instance.Nova.TransitionTrigger.TriggerEnabled = true;
                    }
                    GameManager.Instance.Nova.TransitionTrigger = this;
                    TriggerEnabled = false;
                    GameManager.Instance.Nova.BeginFall();
                }
            }
            else GameManager.Instance.Nova.SwitchFloor(groundFloor);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playFallAnimation && GameManager.Instance.Nova.DoFall)
        {
            switch (direction)
            {
                case TransitionDirection.North:
                    break;
                case TransitionDirection.NorthEast:
                    break;
                case TransitionDirection.East:
                    if (GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity.x < 0)
                    {
                        GameManager.Instance.Nova.StopFall(true);
                    }
                    break;
                case TransitionDirection.SouthEast:
                    break;
                case TransitionDirection.South:
                    if (GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity.y > 0)
                    {
                        if (GameManager.Instance.Nova.TransitionTrigger == null || (GameManager.Instance.Nova.TransitionTrigger != null && GameManager.Instance.Nova.TransitionTrigger != this))
                        {
                            GameManager.Instance.Nova.StopFall(true);
                        }
                    }
                    break;
                case TransitionDirection.SouthWest:
                    break;
                case TransitionDirection.West:
                    if (GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity.x > 0)
                    {
                        GameManager.Instance.Nova.StopFall(true);
                    }
                    break;
                case TransitionDirection.NorthWest:
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
}

public enum TransitionDirection
{
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest
}
