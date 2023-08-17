using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovementTrigger : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The direction the cat will move in
    /// </summary>
    [SerializeField] private Vector2 direction;

    /// <summary>
    /// Reference to the moving cat in the ending
    /// </summary>
    [SerializeField] private FakeLuna fakeLuna;

    /// <summary>
    /// Collision that will be activated to prevent Nova from going back
    /// </summary>
    [SerializeField] private GameObject activatableBarrier;

    /// <summary>
    /// Reference to the 
    /// </summary>
    [SerializeField] private TextboxTrigger preFinalDialog;
    #endregion

    #region Methods
    /// <summary>
    /// Starts moving the cat
    /// </summary>
    private void StartCatMovement()
    {
        fakeLuna.StartMoving(direction);
    }

    /// <summary>
    /// Stops Nova for a short moment after they enter the trigger
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayNovasMovement()
    {
        GameManager.Instance.IsSceneIntroPlaying = true;
        GameManager.Instance.Nova.Animator.SetFloat("velocityX", 0);
        GameManager.Instance.Nova.Animator.SetFloat("velocityY", 0);

        if (activatableBarrier != null) activatableBarrier.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.IsSceneIntroPlaying = false;

        gameObject.SetActive(false);
    }
    #endregion

    #region Unity Stuff

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (preFinalDialog != null)
        {
            preFinalDialog.LoadDialog();
        }
        else
        {
            StartCatMovement();
            StartCoroutine(DelayNovasMovement());
        }
    }
    #endregion
}
