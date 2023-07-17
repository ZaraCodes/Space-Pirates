using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScrollingBackground : MonoBehaviour
{
    #region Fields
    [SerializeField] private Material starsMat;
    [SerializeField] private Material dustMat;
    [SerializeField] private Material nebulaeMat;

    private Vector2 offset;

    #endregion

    #region Methods

    #endregion

    #region Unity Stuff
    private void Start()
    {
        offset = new();
    }
    private void Update()
    {
        offset.x += Time.deltaTime * 0.001f;
        offset.y += Time.deltaTime * 0.2f * 0.001f;

        starsMat.SetTextureOffset("_MainTex", offset);
        dustMat.SetTextureOffset("_MainTex", offset * 2f);
        nebulaeMat.SetTextureOffset("_MainTex", offset * 4f);
        //starsMaterial.SetTextureOffset( starsMaterial.GetTextureOffset(starsMaterial.name)
    }

    private void OnDestroy()
    {
        starsMat.SetTextureOffset("_MainTex", new());
        dustMat.SetTextureOffset("_MainTex", new());
        nebulaeMat.SetTextureOffset("_MainTex", new());
    }
    #endregion
}
