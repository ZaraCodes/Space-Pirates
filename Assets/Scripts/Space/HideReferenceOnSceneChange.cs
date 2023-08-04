using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideReferenceOnSceneChange : MonoBehaviour
{
    public GameObject Reference { get; set; }

    private void OnDestroy()
    {
        if (Reference != null) Reference.SetActive(false);
    }
}
