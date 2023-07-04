using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToTitleButton : MonoBehaviour
{
    public void GoBackToTitle()
    {
        Time.timeScale = 1f;
        GameManager.Instance.IsPlaying = true;
        ChargedBullet.playDestroySound = false;
        SceneManager.LoadScene("Main Menu");
    }
}
