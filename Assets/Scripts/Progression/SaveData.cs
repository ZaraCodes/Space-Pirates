using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    #region Fields
    public List<EProgressionFlag> Flags;
    public List<string> ViewedDialogs;
    public ELastVisitedLocation LastVisitedLocation;
    public bool InOrbit;
    #endregion
}
