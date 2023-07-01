using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : ToggleObject
{
    #region Methods

    #endregion

    #region Unity Stuff

    private void OnTriggerEnter2D(Collider2D collision)
    {
        State = !State;
    }


    #endregion
}
