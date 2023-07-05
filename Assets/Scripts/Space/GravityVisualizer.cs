using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject gravityDot;
    [SerializeField] private GravityReceiver gravityReceiver;

    [SerializeField] private AnimationCurve curve;

    [SerializeField] private bool calcucaleDots;
    [SerializeField] private GameObject dotsContainer;

    private Mesh mesh;

    public void SpawnGravityVisualizer()
    {
        if (!calcucaleDots) return;
        //float c1 = 0.04f;
        //float c2 = 150f;
        //float m = 150f;

        int limit = 500;
        int stepSize = 5;

        // Vector3[] verts = new Vector3[(limit * 2) / stepSize + 1];
        // maybe later

        for (int i = -limit + (int) transform.position.x; i <= limit + (int)transform.position.x; i += stepSize)
        {
            for (int j = -limit + (int)transform.position.y; j <= limit + (int)transform.position.y; j += stepSize)
            {
                GameObject dot = Instantiate(gravityDot);

                float posZ = gravityReceiver.CalculateForce(new(i, j)).magnitude * 40;

                // float result = 1 / (1 + Mathf.Exp(-c1 * (posZ - c2))) * m + 1 / (1 + Mathf.Exp(-c1 * (-posZ + c2))) * posZ;
                //float result = Mathf.Log(posZ * c1 + 1) * m;
                float result = curve.Evaluate(posZ);


                dot.transform.position = new(i, j, result);
                dot.transform.SetParent(dotsContainer.transform);

                float newScale = result / 6000 + dot.transform.localScale.x;
                dot.transform.localScale = new Vector3(newScale, newScale, 1);
                
                dot.isStatic = true;
            }
        }

        // mesh.Clear();
    }

    

    private void Awake()
    {
        SpawnGravityVisualizer();
    }
}
