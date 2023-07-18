using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : ToggleObject
{
    [SerializeField] private int objectCount;
    [SerializeField] private List<GameObject> buttonTriggers;
    #region Unity Stuff
    private void Start()
    {
        objectCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Button Trigger"))
        {
            if (!buttonTriggers.Contains(collision.gameObject))
            {
                buttonTriggers.Add(collision.gameObject);
                objectCount++;
                if (!State) State = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Button Trigger"))
        {
            if (buttonTriggers.Contains(collision.gameObject))
            {
                buttonTriggers.Remove(collision.gameObject);
                objectCount--;
                if (objectCount <= 0)
                {
                    State = false;
                    objectCount = 0;
                }
            }

        }
    }
    #endregion
}
