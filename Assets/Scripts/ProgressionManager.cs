using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class ProgressionManager
{
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

    private ProgressionManager()
    {
        ViewedDialogs = new();
        Flags = new();
    }

    public void LoadProgress()
    {
        //todo
    }
}
