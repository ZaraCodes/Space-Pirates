using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows Nova to get moved automatically to specific positions
/// </summary>
public class CutsceneMovement : MonoBehaviour
{
    #region Fields
    /// <summary>A list of targets to move to</summary>
    public CutsceneMovementTarget[] Targets;
    /// <summary>A unity event that gets triggered once the movement to the last target has been completed</summary>
    public UnityEvent OnMovementFinished;

    /// <summary>Index of the current target</summary>
    private int targetIndex;
    /// <summary>The current target that Nova tries to move to</summary>
    private CutsceneMovementTarget currentTarget;
    /// <summary>Keeps track if Nova reached the x position of the target</summary>
    private bool xReached;
    /// <summary>Keeps track if Nova reached the y position of the target</summary>
    private bool yReached;

    /// <summary>The position Nova started from for the current target</summary>
    private Vector3 startPosition;
    #endregion

    #region Methods
    /// <summary>Activates the cutscene movement</summary>
    public void Activate()
    {
        targetIndex = 0;
        xReached = false;
        yReached = false;
        startPosition = GameManager.Instance.Nova.transform.position;
        GameManager.Instance.Nova.CutsceneMovement = true;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Concludes the movement for the current target. If it is the last target in the array, the event OnMovementFinished gets invoked. If not, the movement towards the next target starts.
    /// </summary>
    public void FinishMovement()
    {
        if (targetIndex < Targets.Length - 1)
        {
            targetIndex++;
            currentTarget = Targets[targetIndex];
            xReached = false;
            yReached = false;
            startPosition = GameManager.Instance.Nova.transform.position;
        }
        else
        {
            GameManager.Instance.Nova.Animator.SetFloat("velocityX", 0);
            GameManager.Instance.Nova.Animator.SetFloat("velocityY", 0);
            GameManager.Instance.Nova.CutsceneMovement = false;
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            OnMovementFinished?.Invoke();
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Approaches the x value of the target position
    /// </summary>
    private void ApproachX()
    {
        float diffA = currentTarget.Target.x - GameManager.Instance.Nova.transform.position.x;
        float diffB = currentTarget.Target.x - startPosition.x;
        if (diffA > 0 && diffB > 0)
        {
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = new Vector3(GameManager.Instance.Nova.MovementSpeed * Time.fixedDeltaTime, 0);
            GameManager.Instance.Nova.Animator.SetFloat("velocityY", 0);
            GameManager.Instance.Nova.Animator.SetFloat("velocityX", 1);
        }
        else if (diffA <= 0 && diffB > 0)
        {
            xReached = true;
            GameManager.Instance.Nova.transform.position = new Vector3(currentTarget.Target.x, GameManager.Instance.Nova.transform.position.y);
        }
        else if (diffA <= 0 && diffB <= 0)
        {
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = new Vector3(-GameManager.Instance.Nova.MovementSpeed * Time.fixedDeltaTime, 0);
            GameManager.Instance.Nova.Animator.SetFloat("velocityY", 0);
            GameManager.Instance.Nova.Animator.SetFloat("velocityX", -1);
        }
        else if (diffA > 0 && diffB <= 0)
        {
            xReached = true;
            GameManager.Instance.Nova.transform.position = new Vector3(currentTarget.Target.x, GameManager.Instance.Nova.transform.position.y);
        }
    }
    /// <summary>
    /// Approaches the y value of the target position
    /// </summary>
    private void ApproachY()
    {
        float diffA = currentTarget.Target.y - GameManager.Instance.Nova.transform.position.y;
        float diffB = currentTarget.Target.y - startPosition.y;
        if (diffA > 0 && diffB > 0)
        {
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = new Vector3(0, GameManager.Instance.Nova.MovementSpeed * Time.fixedDeltaTime);
            GameManager.Instance.Nova.Animator.SetFloat("velocityY", 1);
            GameManager.Instance.Nova.Animator.SetFloat("velocityX", 0);
        }
        else if (diffA <= 0 && diffB > 0)
        {
            yReached = true;
            GameManager.Instance.Nova.transform.position = new Vector3(GameManager.Instance.Nova.transform.position.x, currentTarget.Target.y);

        }
        else if (diffA <= 0 && diffB <= 0)
        {
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -GameManager.Instance.Nova.MovementSpeed * Time.fixedDeltaTime);
            GameManager.Instance.Nova.Animator.SetFloat("velocityY", -1);
            GameManager.Instance.Nova.Animator.SetFloat("velocityX", 0);
        }
        else if (diffA > 0 && diffB <= 0)
        {
            yReached = true;
            GameManager.Instance.Nova.transform.position = new Vector3(GameManager.Instance.Nova.transform.position.x, currentTarget.Target.y);
        }
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// While the game object is active, it executes the movement code until it terminates
    /// </summary>
    private void Update()
    {
        currentTarget = Targets[targetIndex];
        if (currentTarget.PrioritizedAxis == EPrioritizedAxis.X)
        {
            if (!xReached) ApproachX();
            else if (!yReached) ApproachY();
            else FinishMovement();            
        }
        else if (currentTarget.PrioritizedAxis == EPrioritizedAxis.Y)
        {
            if (!yReached) ApproachY();
            else if (!xReached) ApproachX();
            else FinishMovement();
        }
    }
    #endregion

}
