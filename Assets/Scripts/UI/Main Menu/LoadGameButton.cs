using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameButton : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the resume button</summary>
    [SerializeField] private GameObject resumeButton;
    #endregion

    #region Methods
    /// <summary>Loads the save file</summary>
    public void LoadSave()
    {
        ProgressionManager.Instance.LoadProgress();
    }
    #endregion

    #region Unity Stuff
    /// <summary>Hides the resume button if no save file exists</summary>
    private void Start()
    {
        if (!ReaderWriter.DoesFileExist($"{Application.persistentDataPath}\\Data\\Saves\\save.json"))
        {
            resumeButton.SetActive(false);
        }
    }
    #endregion
}
