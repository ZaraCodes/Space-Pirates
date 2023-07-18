using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructionScript : MonoBehaviour
{
    #region Fields
    public float Lifetime { get; set; }

    public delegate void TimerEnded();
    public event TimerEnded OnTimerEnded;
    #endregion

    #region Unity Stuff
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
