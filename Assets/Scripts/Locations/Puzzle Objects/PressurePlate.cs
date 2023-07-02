using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : ToggleObject
{
    [SerializeField] private int objectCount;
    #region Unity Stuff
    private void Start()
    {
        objectCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Button Trigger"))
        {
            objectCount++;
            if (!State) State = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Button Trigger"))
        {
            objectCount--;
            if (objectCount <= 0)
            {
                State = false;
                objectCount = 0;
            }
        }
    }
    #endregion
}
