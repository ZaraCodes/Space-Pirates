using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Spawns points in space to indicate gravity
/// </summary>
public class GravityVisualizer : MonoBehaviour
{
    /// <summary>Reference to the GameObject that contains the gravity dots. This is used to cache the points between scenes so it doesn't have to be calculated every time the player enters the space scene</summary>
    private static GameObject instance;

    /// <summary>Prefab of the gravity dot</summary>
    [SerializeField] private GameObject gravityDot;
    /// <summary>Reference to a gravity receiver to calculate the effects of gravity for all the gravity dots</summary>
    [SerializeField] private GravityReceiver gravityReceiver;
    /// <summary>Defines how far the gravity dots get displaced depending on the force</summary>
    [SerializeField] private AnimationCurve curve;

    /// <summary>should the gravity dots be calculated</summary>
    [SerializeField] private bool calcucaleDots;
    /// <summary>/// Reference to the object that contains the gravity dots</summary>
    [SerializeField] private GameObject dotsContainer;

    /// <summary>Size of the area for gravity dots</summary>
    [SerializeField] private int limit;

    /// <summary>should the dots be cached. this is required because the intro generates dots that will not be saved</summary>
    [SerializeField] private bool cacheDots;

    /// <summary>
    /// Coroutine that generates the gravity dots
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnGravityVisualizer()
    {
        if (calcucaleDots)
        {
            int stepSize = 5;

            int loopBreakerCounter = 0;
            yield return null;
            for (int i = -limit + (int)transform.position.x; i <= limit + (int)transform.position.x; i += stepSize)
            {
                for (int j = -limit + (int)transform.position.y; j <= limit + (int)transform.position.y; j += stepSize)
                {
                    GameObject dot = Instantiate(gravityDot);

                    float posZ = gravityReceiver.CalculateForce(new(i, j)).magnitude * 40;
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
            GameManager.Instance.PauseMenuHandler.LoadingScreen.gameObject.SetActive(false);
            StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeOut(null));
        }
    }

    /// <summary>
    /// Creates an objet that hides the gravity dots when the scene changes
    /// </summary>
    private void CreateHiderObject()
    {
        GameObject hiderGO = new();
        hiderGO.name = "Gravity Dots Hider";
        var hider = hiderGO.AddComponent<HideReferenceOnSceneChange>();
        hider.Reference = instance;
    }

    /// <summary>
    /// Starts generating the gravity dots if the scene is the intro or the instance is null
    /// </summary>
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Intro")
        {
            Destroy(instance);
            instance = null;
        }
        GameManager.Instance.PauseMenuHandler.BlackFade.gameObject.SetActive(true);

        if (instance == null)
        {
            if (cacheDots)
            {
                instance = gameObject;

                CreateHiderObject();
                DontDestroyOnLoad(instance);
            }
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
