using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Starts a new game or opens the scene chooser window
/// </summary>
public class NewGameButton : MonoBehaviour
{
    /// <summary>Reference to the scene chooser</summary>
    [SerializeField] private GameObject sceneChooser;
    /// <summary>Reference to the main menu</summary>
    [SerializeField] private GameObject mainMenu;
    
    /// <summary>
    /// Opens the scene chooser window
    /// </summary>
    public void OpenSceneChooser()
    {
        sceneChooser.SetActive(true);
        mainMenu.SetActive(false);
    }

    /// <summary>
    /// Loads the Intro scene
    /// </summary>
    public void StartNewGame()
    {
        SceneManager.LoadScene(5);
    }

    /// <summary>
    /// Resets the menu and a game manager setting
    /// </summary>
    private void Start()
    {
        GameManager.Instance.IsSceneIntroPlaying = false;
        GetComponent<Button>().Select();
    }
}
