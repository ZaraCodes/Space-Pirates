using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CreditsEnding : MonoBehaviour
{
    /// <summary>
    /// A unity event that gets invoked after the credits have finished scrolling.
    /// </summary>
    [SerializeField] private UnityEvent onCreditsFinished;

    /// <summary>
    /// Loads the main menu
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Triggers when the credits have finished
    /// </summary>
    /// <param name="collision">Collision of the other object (unused)</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        onCreditsFinished.Invoke();
    }
}
