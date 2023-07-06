using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletContainer : MonoBehaviour
{
    private void OnDestroy()
    {
        ChargedBullet.playDestroySoundStatic = false;
    }
}
