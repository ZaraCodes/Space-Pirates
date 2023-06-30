using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    [SerializeField] private Button spaceButton;
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    private void OnEnable()
    {
        spaceButton.Select();
    }
}
