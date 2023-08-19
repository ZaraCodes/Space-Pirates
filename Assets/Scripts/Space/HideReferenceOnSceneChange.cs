using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Hides the referenced object when this gets destroyed</summary>
public class HideReferenceOnSceneChange : MonoBehaviour
{
    /// <summary>The referenced object that will be hidden on destroy</summary>
    public GameObject Reference { get; set; }

    /// <summary>
    /// Hides the referenced object when this gets destroyed
    /// </summary>
    private void OnDestroy()
    {
        if (Reference != null) Reference.SetActive(false);
    }
}
