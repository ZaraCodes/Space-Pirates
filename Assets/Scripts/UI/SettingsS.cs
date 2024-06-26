using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Singleton that contains the various settings of the game
/// </summary>
public class SettingsS
{
    /// <summary>private instance of the singleton</summary>
    private static SettingsS instance;

    /// <summary>
    /// Property that ensures that this is a singleton
    /// </summary>
    public static SettingsS Instance
    {
        get
        {
            instance ??= new SettingsS();
            return instance;
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    private SettingsS()
    {
        UIScale = 3;
        SoundVolume = .5f;
        MusicVolume = .5f;
        MasterVolume = .5f;
        TextboxSpeed = 60f;
        //Todo: Load Settings from File or default settings
    }


    /// <summary>The current language</summary>
    public int CurrentLocaleIndex { get; set; }

    /// <summary>The current Sound Volume</summary>
    public float SoundVolume { get; set; }

    /// <summary>The current Music Volume</summary>
    public float MusicVolume { get; set; }

    /// <summary>The current Master Volume</summary>
    public float MasterVolume { get; set; }
    
    /// <summary>Defines if the HP Bar should always be shown (true) or only when it's relevant (false)</summary>
    public bool AlwaysShowHP { get; set; }

    /// <summary>Sets how fast text will scroll</summary>
    public float TextboxSpeed { get; set; }

    /// <summary>The scale of the UI Canvas</summary>
    public float UIScale { get; set; }
}
