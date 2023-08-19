using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A floor transition is used to transition between the ground floor and the first floor and vice versa.
/// </summary>
public class FloorTransition : MonoBehaviour
{
    #region Fields
    /// <summary>The direction of the transition</summary>
    [SerializeField] private ETransitionDirection direction;
    /// <summary>True if the transition makes Nova go to the ground floor, false if they go to the first floor</summary>
    [SerializeField] private bool groundFloor;
    /// <summary>True if Nova should play the fall/jump animation</summary>
    [SerializeField] private bool playFallAnimation;

    /// <summary>Keeps track of the current state of the trigger. This is required because Nova would enter the trigger multiple times depending on the direction of the jump</summary>
    public bool TriggerEnabled;
    #endregion

    #region Unity Stuff
    private void Start()
    {
        TriggerEnabled = true;
    }

    /// <summary>
    /// If Nova triggers this, they will either switch the floor or jump
    /// </summary>
    /// <param name="collision">The collider of the other object</param>
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

                    if (GameManager.Instance.Nova.TransitionTrigger != null && GameManager.Instance.Nova.TransitionTrigger != this)
                    {
                        GameManager.Instance.Nova.TransitionTrigger.TriggerEnabled = true;
                    }
                    GameManager.Instance.Nova.TransitionTrigger = this;
                    TriggerEnabled = false;
                    
                    if (!(direction == ETransitionDirection.North && GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity.y < 0) && 
                        !(direction == ETransitionDirection.South && GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity.y > 0))
                        GameManager.Instance.Nova.BeginFall();
                }
            }
            else GameManager.Instance.Nova.SwitchFloor(groundFloor);
        }
    }

    /// <summary>
    /// If during a fall/jump Nova exits a transition trigger, depending on the direction of movement, the direction of the trigger they can also land on the top floor
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playFallAnimation && GameManager.Instance.Nova.DoFall)
        {
            switch (direction)
            {
                case ETransitionDirection.North:
                    if (GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity.y < 0)
                    {
                        GameManager.Instance.Nova.StopFall(true);
                    }
                    break;
                case ETransitionDirection.NorthEast:
                    break;
                case ETransitionDirection.East:
                    if (GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity.x < 0)
                    {
                        GameManager.Instance.Nova.StopFall(true);
                    }
                    break;
                case ETransitionDirection.SouthEast:
                    break;
                case ETransitionDirection.South:
                    if (GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity.y > 0)
                    {
                        if (GameManager.Instance.Nova.TransitionTrigger == null || (GameManager.Instance.Nova.TransitionTrigger != null && GameManager.Instance.Nova.TransitionTrigger != this))
                        {
                            GameManager.Instance.Nova.StopFall(true);
                        }
                    }
                    break;
                case ETransitionDirection.SouthWest:
                    break;
                case ETransitionDirection.West:
                    if (GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity.x > 0)
                    {
                        GameManager.Instance.Nova.StopFall(true);
                    }
                    break;
                case ETransitionDirection.NorthWest:
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
}

/// <summary>
/// Enum that has all transition directions, even though not all ended up used
/// </summary>
public enum ETransitionDirection
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
