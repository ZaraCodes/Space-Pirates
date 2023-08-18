using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Credits : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the object that scrolls the credits</summary>
    [SerializeField] private GameObject scrollObject;
    /// <summary>If true, the credits will show the ending text</summary>
    [SerializeField] private bool showEndGameText;
    /// <summary>Reference to the ending text</summary>
    [SerializeField] private TextMeshProUGUI endingText;
    /// <summary>Time for how long the credits will wait until it starts scrolling</summary>
    [SerializeField] private float waitDuration;
    /// <summary>The scrolling speed of the credits</summary>
    [SerializeField] private float scrollSpeed;

    private bool scrolling;
    #endregion

    #region Methods
    /// <summary>Coroutine that scrolls the credits</summary>
    /// <returns></returns>
    private IEnumerator ScrollCredits()
    {
        yield return new WaitForSeconds(waitDuration);
        scrolling = true;
        while (scrolling)
        {
            yield return null;
            scrollObject.transform.position = new(scrollObject.transform.position.x, scrollObject.transform.position.y + Time.deltaTime * scrollSpeed);
        }
    }

    /// <summary>
    /// Resets the credits position
    /// </summary>
    public void ResetPosition()
    {
        scrollObject.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Stops the credits movement 
    /// </summary>
    public void StopCredits()
    {
        scrolling = false;
    }

    #endregion

    #region Unity Stuff
    private void OnEnable()
    {
        if (!showEndGameText) endingText.color = new Color(0, 0, 0, 0);
        StartCoroutine(ScrollCredits());
    }
    #endregion
}
