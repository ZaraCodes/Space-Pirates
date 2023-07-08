using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToTitleButton : MonoBehaviour
{
    public void GoBackToTitle()
    {
        Time.timeScale = 1f;
        GameManager.Instance.IsPauseMenuOpen = false;
        GameManager.Instance.IsDialogOverlayOpen = false;
        ChargedBullet.playDestroySoundStatic = false;
        SceneManager.LoadScene("Main Menu");
    }
}
