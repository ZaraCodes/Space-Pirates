using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is relevant when using two floors with Nova. It disables collisions of first floor barriers to make a smoother jump possible.
/// </summary>
public class CollisionManager : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// This array contains all sets of collisions from the first floor sections of the scene
    /// </summary>
    [SerializeField] private GameObject[] collisionContainer;
    #endregion

    #region Methods
    /// <summary>
    /// Enables the collisions 
    /// </summary>
    public void EnableCollisions()
    {
        foreach (var item in collisionContainer)
        {
            item.SetActive(true);
        }
    }

    /// <summary>
    /// Disables the collisions
    /// </summary>
    public void DisableCollisions()
    {
        foreach (var item in collisionContainer)
        {
            item.SetActive(false);
        }
    }
    #endregion
}
