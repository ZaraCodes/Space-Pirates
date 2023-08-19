using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class contains the data needed for a save 
/// </summary>
[Serializable]
public class SaveData
{
    #region Fields
    /// <summary>The list of progression flags</summary>
    public List<EProgressionFlag> Flags;
    /// <summary>The list of viewed dialogs</summary>
    public List<string> ViewedDialogs;
    /// <summary>The last visited location</summary>
    public ELastVisitedLocation LastVisitedLocation;
    /// <summary>True if the ship is in orbit. False if the player is currently in a location</summary>
    public bool InOrbit;
    #endregion
}
