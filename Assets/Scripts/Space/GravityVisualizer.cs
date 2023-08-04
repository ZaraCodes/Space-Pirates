using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GravityVisualizer : MonoBehaviour
{
    private static GameObject instance;

    [SerializeField] private GameObject gravityDot;
    [SerializeField] private GravityReceiver gravityReceiver;

    [SerializeField] private AnimationCurve curve;

    [SerializeField] private bool calcucaleDots;
    [SerializeField] private GameObject dotsContainer;

    [SerializeField] private int limit;

    public IEnumerator SpawnGravityVisualizer()
    {
        if (calcucaleDots)
        {
            //float c1 = 0.04f;
            //float c2 = 150f;
            //float m = 150f;

            int stepSize = 5;

            // Vector3[] verts = new Vector3[(limit * 2) / stepSize + 1];
            // maybe later
            int loopBreakerCounter = 0;
            yield return null;
            for (int i = -limit + (int)transform.position.x; i <= limit + (int)transform.position.x; i += stepSize)
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
                if (++loopBreakerCounter == 4)
                {
                    loopBreakerCounter = 0;
                    yield return null;
                }
            }
            //yield return new WaitForSeconds(.5f);
            GameManager.Instance.PauseMenuHandler.LoadingScreen.gameObject.SetActive(false);
            StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeOut(null));
        }
    }

    private void CreateHiderObject()
    {
        GameObject hiderGO = new();
        var hider = hiderGO.AddComponent<HideReferenceOnSceneChange>();
        hider.Reference = instance;
    }

    private void Start()
    {
        GameManager.Instance.PauseMenuHandler.BlackFade.gameObject.SetActive(true);

        if (instance == null)
        {
            instance = gameObject;
            
            CreateHiderObject();
            DontDestroyOnLoad(instance);

            GameManager.Instance.PauseMenuHandler.LoadingScreen.gameObject.SetActive(true);
            GameManager.Instance.PauseMenuHandler.BlackFade.color = Color.black;
            StartCoroutine(SpawnGravityVisualizer());
        }
        else
        {
            CreateHiderObject();
            instance.SetActive(true);

            GameManager.Instance.PauseMenuHandler.BlackFade.color = Color.black;

            UnityEvent unityEvent = new();
            unityEvent.AddListener(() => Destroy(gameObject));
            StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeOut(unityEvent));
        }
    }
}
