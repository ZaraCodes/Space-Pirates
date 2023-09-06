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

    /// <summary>
    /// Loads the save data from the given path
    /// </summary>
    /// <param name="path">The path from where the save data will be loaded from</param>
    /// <returns>SaveData object or null if something goes wrong</returns>
    public static SaveData LoadFile(string path)
    {
        if (Directory.Exists(path))
        {
            var jsonSave = File.ReadAllText($"{path}\\save.json");
            try
            {
                return JsonUtility.FromJson<SaveData>(jsonSave);
            }
            catch 
            {
                return null;
            }
        }
        return null;
    }

    /// <summary>
    /// Deletes a file from the given path
    /// </summary>
    /// <param name="path">The path to the file that will be deleted</param>
    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// Checks if a file exists
    /// </summary>
    /// <param name="path">The path to the file that will be checked for existance</param>
    /// <returns></returns>
    public static bool DoesFileExist(string path)
    {
        return File.Exists(path);
    }
}
