using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This object is used to have all bullets in a scene as child objects of this so it is more ordered.
/// </summary>
public class BulletContainer : MonoBehaviour
{
    /// <summary>
    /// When the scene gets changed (and this object gets destroyed), it prevents charged bullets from emitting a sound because they are getting destroyed too
    /// </summary>
    private void OnDestroy()
    {
        ChargedBullet.PlayDestroySoundStatic = false;
    }
}
