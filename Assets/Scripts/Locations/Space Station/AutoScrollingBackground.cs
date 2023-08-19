using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The background uses four different layers to create a parallax effect that's best viewed with a ortographic camera angle
/// </summary>
public class AutoScrollingBackground : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the stars material (layer)</summary>
    [SerializeField] private Material starsMat;
    /// <summary>Reference to the dust material (layer)</summary>
    [SerializeField] private Material dustMat;
    /// <summary>Reference to the nebulae material (layer)</summary>
    [SerializeField] private Material nebulaeMat;

    /// <summary>the animation offset for the layers</summary>
    private Vector2 offset;
    #endregion

    #region Unity Stuff
    /// <summary>Sets the offset vector</summary>
    private void Start()
    {
        offset = new();
    }
    /// <summary>
    /// Each frame the layers get moved
    /// </summary>
    private void Update()
    {
        offset.x += Time.deltaTime * 0.001f;
        offset.y += Time.deltaTime * 0.2f * 0.001f;

        starsMat.SetTextureOffset("_MainTex", offset);
        dustMat.SetTextureOffset("_MainTex", offset * 2f);
        nebulaeMat.SetTextureOffset("_MainTex", offset * 4f);
        //starsMaterial.SetTextureOffset( starsMaterial.GetTextureOffset(starsMaterial.name)
    }

    /// <summary>
    /// Resets the offsets for the materials when the object gets destroyed
    /// </summary>
    private void OnDestroy()
    {
        starsMat.SetTextureOffset("_MainTex", new());
        dustMat.SetTextureOffset("_MainTex", new());
        nebulaeMat.SetTextureOffset("_MainTex", new());
    }
    #endregion
}
