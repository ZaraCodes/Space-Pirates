using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Contains required information for the scene chooser
/// </summary>
public class SceneSelector : MonoBehaviour
{
    /// <summary>
    /// A button to select
    /// </summary>
    [SerializeField] private Button spaceButton;

    /// <summary>
    /// Loads the scene
    /// </summary>
    /// <param name="scene">The name of the scene that will be loaded</param>
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// Selects the space button when the window gets enabled
    /// </summary>
    private void OnEnable()
    {
        spaceButton.Select();
    }
}
