using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>A pressure plate is a toggle object that gets toggled when Nova or a movable box are on top of it</summary>
public class PressurePlate : ToggleObject
{
    #region Fields
    /// <summary>The object count keeps track of how many objects are on top of the pressure plate. This is required to calculate when to execute the toggle event</summary>
    [SerializeField] private int objectCount;
    /// <summary>The list of objects that currently are on the pressure plate</summary>
    [SerializeField] private List<GameObject> buttonTriggers;
    #endregion

    #region Unity Stuff
    /// <summary>Sets the object count to 0 at the start of the scene</summary>
    private void Start()
    {
        objectCount = 0;
    }

    /// <summary>
    /// When an object enters this trigger, it will increase the object count and execute a state change if the conditions are met
    /// </summary>
    /// <param name="collision">The collider of the other object</param>
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

    /// <summary>When an object leaves this trigger, the object count gets reduced and a state change occurs if the conditions are met</summary>
    /// <param name="collision">The collider of the other object</param>
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
