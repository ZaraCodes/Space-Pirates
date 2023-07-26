using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject[] collisionContainer;
    #endregion

    #region Methods
    public void EnableCollisions()
    {
        foreach (var item in collisionContainer)
        {
            item.SetActive(true);
        }
    }

    public void DisableCollisions()
    {
        foreach (var item in collisionContainer)
        {
            item.SetActive(false);
        }
    }
    #endregion
}
