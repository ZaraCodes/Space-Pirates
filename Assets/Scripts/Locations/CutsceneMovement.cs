using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneMovement : MonoBehaviour
{
    #region Fields
    public CutsceneMovementTarget[] Targets;
    public UnityEvent OnMovementFinished;

    private int targetIndex;
    private CutsceneMovementTarget currentTarget;
    private bool xReached;
    private bool yReached;

    private Vector3 startPosition;
    #endregion

    #region Methods
    public void Activate()
    {
        targetIndex = 0;
        xReached = false;
        yReached = false;
        startPosition = GameManager.Instance.Nova.transform.position;
        GameManager.Instance.Nova.CutsceneMovement = true;
        gameObject.SetActive(true);
    }

    public void FinishMovement()
    {
        if (targetIndex < Targets.Length - 1)
        {
            targetIndex++;
            currentTarget = Targets[targetIndex];
            xReached = false;
            yReached = false;
        }
        else
        {
            GameManager.Instance.Nova.CutsceneMovement = false;
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            OnMovementFinished?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void ApproachX()
    {
        float diffA = currentTarget.Target.x - GameManager.Instance.Nova.transform.position.x;
        float diffB = currentTarget.Target.x - startPosition.x;
        if (diffA > 0 && diffB > 0)
        {
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = new Vector3(GameManager.Instance.Nova.MovementSpeed * Time.fixedDeltaTime, 0);
        }
        else if (diffA <= 0 && diffB > 0)
        {
            xReached = true;
            GameManager.Instance.Nova.transform.position = new Vector3(currentTarget.Target.x, GameManager.Instance.Nova.transform.position.y);
        }
        else if (diffA <= 0 && diffB <= 0)
        {
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = new Vector3(-GameManager.Instance.Nova.MovementSpeed * Time.fixedDeltaTime, 0);
        }
        else if (diffA > 0 && diffB <= 0)
        {
            xReached = true;
            GameManager.Instance.Nova.transform.position = new Vector3(currentTarget.Target.x, GameManager.Instance.Nova.transform.position.y);
        }
    }

    private void ApproachY()
    {
        float diffA = currentTarget.Target.y - GameManager.Instance.Nova.transform.position.y;
        float diffB = currentTarget.Target.y - startPosition.y;
        if (diffA > 0 && diffB > 0)
        {
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = new Vector3(0, GameManager.Instance.Nova.MovementSpeed * Time.fixedDeltaTime);
        }
        else if (diffA <= 0 && diffB > 0)
        {
            yReached = true;
            GameManager.Instance.Nova.transform.position = new Vector3(GameManager.Instance.Nova.transform.position.x, currentTarget.Target.y);

        }
        else if (diffA <= 0 && diffB <= 0)
        {
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -GameManager.Instance.Nova.MovementSpeed * Time.fixedDeltaTime);
        }
        else if (diffA > 0 && diffB <= 0)
        {
            yReached = true;
            GameManager.Instance.Nova.transform.position = new Vector3(GameManager.Instance.Nova.transform.position.x, currentTarget.Target.y);
        }
    }
    #endregion

    #region Unity Stuff
    private void Update()
    {
        currentTarget = Targets[targetIndex];
        if (currentTarget.PrioritizedAxis == PrioritizedAxis.X)
        {
            if (!xReached) ApproachX();
            else if (!yReached) ApproachY();
            else FinishMovement();            
        }
        else if (currentTarget.PrioritizedAxis == PrioritizedAxis.Y)
        {
            if (!yReached) ApproachY();
            else if (!xReached) ApproachX();
            else FinishMovement();
        }
    }
    #endregion

}
