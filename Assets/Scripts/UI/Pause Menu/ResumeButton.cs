using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resume button from the pause menu
/// </summary>
public class ResumeButton : MonoBehaviour
{
    /// <summary>
    /// Reference to the pause menu
    /// </summary>
    [SerializeField] private GameObject pauseMenu;

    /// <summary>
    /// Closes the pause menu
    /// </summary>
    public void Unpause()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Instance.IsPauseMenuOpen = false;
    }
}
