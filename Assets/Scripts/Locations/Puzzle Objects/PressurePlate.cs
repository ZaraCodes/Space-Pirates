using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : ToggleObject
{
    private int objectCount;
    #region Unity Stuff
    private void Start()
    {
        objectCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectCount++;
        State = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectCount--;
        if (objectCount == 0) State = false;
    }
    #endregion
}
