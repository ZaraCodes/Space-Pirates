using System.IO;
using UnityEngine;

/// <summary>
/// This is resposible for reading and writing save data
/// </summary>
public class ReaderWriter
{
    /// <summary>
    /// Checks if the given path exists and creates it if it does not exist
    /// </summary>
    /// <param name="pathToFolder">the path to check</param>
    public static void CreateFolderIfNotExists(string pathToFolder)
    {
        if (!Directory.Exists(pathToFolder))
        {
            Directory.CreateDirectory(pathToFolder);
        }
    }

    /// <summary>
    /// Saves the save data at the given path
    /// </summary>
    /// <param name="saveData">The save data to be saved</param>
    /// <param name="path">The path at which the save data will be saved</param>
    public static void SaveFile(SaveData saveData, string path)
    {
        if (Directory.Exists(path))
        {
            var jsonSave = JsonUtility.ToJson(saveData, true);
            File.WriteAllText($"{path}\\save.json", jsonSave);
        }
    }
}
