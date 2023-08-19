using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Returns back to the main menu
/// </summary>
public class BackToTitleButton : MonoBehaviour
{
    /// <summary>
    /// returns back to the main menu
    /// </summary>
    public void GoBackToTitle()
    {
        Time.timeScale = 1f;
        ProgressionManager.Instance.ResetProgress();
        GameManager.Instance.IsPauseMenuOpen = false;
        GameManager.Instance.IsDialogOverlayOpen = false;
        ChargedBullet.PlayDestroySoundStatic = false;
        SceneManager.LoadScene("Main Menu");
    }
}
