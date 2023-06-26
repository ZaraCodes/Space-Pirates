using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get 
        {
            instance ??= new();
            return instance;
        }
    }

    public bool IsPlaying { get; set; }

    public bool IsSettingsMenuOpen { get; set; }

    private GameManager()
    {
        //Todo: Load Default Game State
        IsPlaying = true;
        IsSettingsMenuOpen = false;
    }
}
