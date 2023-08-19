using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game object this is attached to will destroy itself when the timer runs out
/// </summary>
public class SelfDestructionScript : MonoBehaviour
{
    #region Fields
    /// <summary>The lifetime of the object</summary>
    public float Lifetime { get; set; }

    /// <summary>Delegate that gets used when the timer ended</summary>
    public delegate void TimerEnded();
    /// <summary>
    /// event that gets invoked when the timer ends and destroys the gameobject
    /// </summary>
    public event TimerEnded OnTimerEnded;
    #endregion

    #region Unity Stuff
    /// <summary>
    /// Ticks down the timer and invokes the event and destroys itself when it reaches 0
    /// </summary>
    private void Update()
    {
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0)
        {
            OnTimerEnded?.Invoke();
            Destroy(gameObject);
        }
    }
    #endregion
}
