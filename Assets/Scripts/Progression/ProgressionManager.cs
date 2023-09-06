using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The Progression Manager tracks the progression of the game
/// </summary>
public class ProgressionManager
{
    #region Fields
    /// <summary>private static instance of the progression manager</summary>
    private static ProgressionManager instance;
    /// <summary>The progression manager is a singleton</summary>
    public static ProgressionManager Instance
    {
        get
        {
            return instance ??= new();
        }
    }

    /// <summary>The list of dialogs that have been viewed which require tracking because they are not supposed to be repeated. Repeating dialogs are not tracked</summary>
    public List<string> ViewedDialogs { get; set; }

    /// <summary>List of progression flags</summary>
    private List<EProgressionFlag> flags;
    /// <summary>List of progression flags</summary>
    public List<EProgressionFlag> Flags
    {
        get
        {
            return flags;
        }
        private set
        {
            flags = value;
        }
    }
    /// <summary>Tracks the last visited location</summary>
    public ELastVisitedLocation LastVisitedLocation;
    #endregion

    #region Methods
    /// <summary>Constructor</summary>
    private ProgressionManager()
    {
        ViewedDialogs = new();
        Flags = new();
    }

    /// <summary>Loads the progression saved from the save file</summary>
    public void LoadProgress()
    {
        var saveDataPath = $"{Application.persistentDataPath}\\Data\\Saves";
        var saveData = ReaderWriter.LoadFile(saveDataPath);

        if (saveData == null) return;

        Flags.Clear();
        Flags = saveData.Flags;
        LastVisitedLocation = saveData.LastVisitedLocation;
        ViewedDialogs.Clear();
        ViewedDialogs = saveData.ViewedDialogs;

        if (!saveData.InOrbit)
        {
            SceneManager.LoadScene(1);
            return;
        }

        switch (LastVisitedLocation)
        {
            case ELastVisitedLocation.SpaceStation:
                SceneManager.LoadScene(3);
                break;
            case ELastVisitedLocation.Island:
                SceneManager.LoadScene(2);
                break;
            case ELastVisitedLocation.City:
                SceneManager.LoadScene(4);
                break;
            case ELastVisitedLocation.Moon:
                SceneManager.LoadScene(6);
                break;
            default:
                SceneManager.LoadScene(1);
                break;
        }
    }

    /// <summary>Saves the current save state to a json file</summary>
    public void SaveProgress()
    {
        SaveData saveData = new()
        {
            Flags = Flags,
            LastVisitedLocation = LastVisitedLocation,
            ViewedDialogs = ViewedDialogs,
            InOrbit = SceneManager.GetActiveScene().buildIndex != 1
        };

        var saveDataPath = $"{Application.persistentDataPath}\\Data\\Saves";
        ReaderWriter.CreateFolderIfNotExists(saveDataPath);
        ReaderWriter.SaveFile(saveData, saveDataPath);
    }

    /// <summary>Resets the current progression</summary>
    public void ResetProgress()
    {
        flags.Clear();
        ViewedDialogs.Clear();
        LastVisitedLocation = ELastVisitedLocation.None;
    }
    
    /// <summary>
    /// Deletes the save file
    /// </summary>
    public void DeleteSaveFile()
    {
        ReaderWriter.DeleteFile($"{Application.persistentDataPath}\\Data\\Saves\\save.json");
    }
    #endregion
}
