using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBoundary : MonoBehaviour
{
    #region Fields
    private static Vector2 originalVelocity;
    [SerializeField] private TextboxTrigger dialog;
    [SerializeField] private HopeShip hopeShip;
    #endregion

    #region Methods
    public void ReverseShipVelocity()
    {
        hopeShip.Velocity = -originalVelocity;
    }

    #endregion

    #region Unity Stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        originalVelocity = hopeShip.Velocity;
        dialog.LoadDialog();
    }
    #endregion
}
