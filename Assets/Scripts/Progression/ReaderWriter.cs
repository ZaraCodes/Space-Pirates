using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReaderWriter
{
    public static void CreateFolderIfNotExists(string pathToFolder)
    {
        if (!Directory.Exists(pathToFolder))
        {
            Directory.CreateDirectory(pathToFolder);
        }
    }
}
