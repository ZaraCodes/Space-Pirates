using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private GameObject sceneChooser;
    [SerializeField] private GameObject mainMenu;
    public void LoadSpaceScene()
    {
        sceneChooser.SetActive(true);
        mainMenu.SetActive(false);
    }

    private void Start()
    {
        GetComponent<Button>().Select();
    }
}
