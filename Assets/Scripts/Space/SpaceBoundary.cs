using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reverses the ship velocity if it moves out of the defined space
/// </summary>
public class SpaceBoundary : MonoBehaviour
{
    #region Fields
    /// <summary>The speed at which the ship enters the trigger</summary>
    private static Vector2 originalVelocity;
    /// <summary>The dialog that gets played when entering the trigger</summary>
    [SerializeField] private TextboxTrigger dialog;
    /// <summary>Reference to the hope ship</summary>
    [SerializeField] private HopeShip hopeShip;
    #endregion

    #region Methods
    /// <summary>
    /// Reverses the ship's velocity
    /// </summary>
    public void ReverseShipVelocity()
    {
        hopeShip.Velocity = -originalVelocity;
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// Reverses the ships velocity when something enters the trigger
    /// </summary>
    /// <param name="collision">Collider of the other object</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        originalVelocity = hopeShip.Velocity;
        dialog.LoadDialog();
    }
    #endregion
}
