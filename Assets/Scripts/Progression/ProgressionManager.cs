using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManager
{
    #region Fields
    private static ProgressionManager instance;
    public static ProgressionManager Instance
    {
        get
        {
            return instance ??= new();
        }
    }

    public List<string> ViewedDialogs { get; set; }

    private List<EProgressionFlag> flags;
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

    public ELastVisitedLocation LastVisitedLocation;
    #endregion

    #region Methods
    private ProgressionManager()
    {
        ViewedDialogs = new();
        Flags = new();
    }

    public void LoadProgress()
    {
        //todo
    }

    public void SaveProgress()
    {
        SaveData saveData = new SaveData()
        {
            Flags = Flags,
            LastVisitedLocation = LastVisitedLocation,
            ViewedDialogs = ViewedDialogs,
            InOrbit = SceneManager.GetActiveScene().buildIndex != 1
        };

        var saveDataPath = $"{Application.persistentDataPath}\\Data\\Saves";
        ReaderWriter.CreateFolderIfNotExists(saveDataPath);
        var jsonSave = JsonUtility.ToJson(saveData, true);
        File.WriteAllText($"{saveDataPath}\\save.json", jsonSave);
    }

    public void ResetProgress()
    {
        flags.Clear();
        ViewedDialogs.Clear();
        LastVisitedLocation = ELastVisitedLocation.None;
    }
    #endregion
}
