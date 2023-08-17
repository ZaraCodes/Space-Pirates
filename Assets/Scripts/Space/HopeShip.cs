using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

public class HopeShip : MonoBehaviour
{
    private SpacePiratesControls controls;

    [SerializeField] private float accelerationForce;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera cam;
    [SerializeField] private GravityReceiver gravityReceiver;
    [SerializeField] private AudioSource thrusterSource;

    [SerializeField] private bool introShip;

    private Coroutine thrusterFadeCoroutine;

    private float maxThrusterVolume;

    private Vector2 lookDirection;

    private Vector2 shipInput;

    private Vector2 velocity;

    public Vector2 Velocity { get { return velocity; } set { velocity = value; } }

    private bool accelerate;

    private Transform shipTransform;

    private float orbitTime;

    private bool orbiting;

    private GravityObject currentPlanet;

    public Vector3 OrbitCenter { get; private set; }

    private float orbitDistance;

    private float orbitDirection;

    [SerializeField] private ParticleSystem thrusterParticles;

    [SerializeField] private InteractionPrompt landPrompt;
    [SerializeField] private InteractionPrompt exitOrbitPrompt;
    [SerializeField] private LocalizedString landText;
    [SerializeField] private LocalizedString exitOrbitText;

    [SerializeField] private Transform spaceStationSpawn;
    [SerializeField] private Transform islandSpawn;
    [SerializeField] private Transform moonSpawn;
    [SerializeField] private Transform citySpawn;

    private bool emergencyMode;

    private float fadeBufferTime;

    #region Methods

    /// <summary>This method toggles the acceleration of the ship depending on the input</summary>
    /// <param name="ctx"></param>
    private void ToggleAcceleration(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (ctx.action.WasPressedThisFrame())
        {
            if (currentPlanet == null)
            {
                if (thrusterFadeCoroutine != null)
                    StopCoroutine(thrusterFadeCoroutine);
                thrusterFadeCoroutine = StartCoroutine(FadeThrusterSourceIn());
                thrusterParticles.Play();
            }
            accelerate = true;
        }
        else if (ctx.action.WasReleasedThisFrame())
        {
            if (thrusterFadeCoroutine != null)
                StopCoroutine(thrusterFadeCoroutine);
            thrusterSource.Stop();
            thrusterParticles.Stop();
            accelerate = false;
        }
    }

    /// <summary>Coroutine that slowly fades in the thruster audio</summary>
    /// <returns></returns>
    private IEnumerator FadeThrusterSourceIn()
    {
        thrusterSource.volume = 0f;
        thrusterSource.Play();
        while (thrusterSource.volume < maxThrusterVolume)
        {
            yield return null;
            thrusterSource.volume += maxThrusterVolume / .4f * Time.deltaTime;
        }
    }

    /// <summary>Reads the input for the space ship orientation direction</summary>
    /// <param name="ctx"></param>
    private void ReadShipDirection(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (GameManager.Instance.IsPlaying)
        {
            shipInput = ctx.ReadValue<Vector2>();
        }
    }

    /// <summary>Moves the space ship along the orbit line of a planet</summary>
    public void MoveInOrbit()
    {
        Vector3 oldPos = shipTransform.position;
        orbitTime += Time.deltaTime / currentPlanet.OrbitSpeed * orbitDirection;
        shipTransform.position = OrbitCenter + new Vector3(Mathf.Sin(orbitTime), Mathf.Cos(orbitTime)) * orbitDistance;

        velocity = -(oldPos - shipTransform.position) / Time.deltaTime;
    }

    /// <summary>Sets up the space ship for movement along the orbit line of a planet</summary>
    /// <param name="center"></param>
    public void InitiateOrbit(Vector3 center)
    {
        thrusterParticles.Stop();
        if (thrusterFadeCoroutine != null)
            StopCoroutine(thrusterFadeCoroutine);
        thrusterSource.Stop();

        OrbitCenter = center;

        orbitDistance = currentPlanet.OrbitDistance;

        Vector3 minDistanceToShip = OrbitCenter + new Vector3(Mathf.Sin(0), Mathf.Cos(0)) * orbitDistance;
        Vector3 shipPos = new(shipTransform.position.x, shipTransform.position.y);

        for (float i = 0; i < Mathf.PI * 2; i += Time.fixedDeltaTime)
        {
            Vector3 newMinDistanceToShip = OrbitCenter + new Vector3(Mathf.Sin(i), Mathf.Cos(i)) * orbitDistance;
            if ((newMinDistanceToShip - shipPos).magnitude < (minDistanceToShip - shipPos).magnitude)
            {
                minDistanceToShip = newMinDistanceToShip;
                orbitTime = i;
            }
        }
        shipTransform.position = minDistanceToShip;

        // decides which direction the space ship will travel along on the orbit line
        Vector3 directionA = minDistanceToShip - OrbitCenter + new Vector3(Mathf.Sin(orbitTime + Time.fixedDeltaTime), Mathf.Cos(orbitTime + Time.fixedDeltaTime)) * orbitDistance;
        Vector3 directionB = minDistanceToShip - OrbitCenter + new Vector3(Mathf.Sin(orbitTime - Time.fixedDeltaTime), Mathf.Cos(orbitTime - Time.fixedDeltaTime)) * orbitDistance;

        if (Vector3.Angle(velocity, directionA) < Vector3.Angle(velocity, directionB)) orbitDirection = 1;
        else orbitDirection = -1;

        rb.velocity = Vector2.zero;
        OrbitCenter = center;
        orbiting = true;
        ShowPlanetPrompts();
    }

    /// <summary>Shows the control prompts for landing and exiting the orbit while in an orbit</summary>
    public void ShowPlanetPrompts()
    {
        landPrompt.EnablePrompt(landText, controls.SpaceShip.Land.bindings);
        exitOrbitPrompt.EnablePrompt(exitOrbitText, controls.SpaceShip.ExitOrbit.bindings);
    }

    /// <summary>Hides the orbit control prompts</summary>
    public void HidePlanetPrompts()
    {
        landPrompt.Hide();
        exitOrbitPrompt.Hide();
    }

    /// <summary>Changes the scene depending on the planet</summary>
    /// <param name="ctx"></param>
    public void Land(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);
        if (ctx.action.WasPerformedThisFrame() && currentPlanet != null && !GameManager.Instance.IsFading)
        {
            ProgressionManager.Instance.LastVisitedLocation = currentPlanet.Location;

            UnityEvent unityEvent = new();
            unityEvent.AddListener(() => StartCoroutine(LoadPlanetAsync()));

            StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(unityEvent));
        }
    }

    /// <summary>
    /// Loads a planet async
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadPlanetAsync()
    {
        var asyncOperation = SceneManager.LoadSceneAsync(currentPlanet.SceneIndexToLoad);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    /// <summary>Removes the space ship from the orbit around a planet</summary>
    /// <param name="ctx"></param>
    public void ExitOrbit(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (ctx.action.WasPerformedThisFrame() && !GameManager.Instance.IsFading)
        {
            orbiting = false;
            HidePlanetPrompts();
            currentPlanet = null;

            if (accelerate)
            {
                thrusterFadeCoroutine = StartCoroutine(FadeThrusterSourceIn());
                thrusterParticles.Play();
            }
        }
    }

    private IEnumerator RespawnShipAfterSunImpact()
    {
        shipTransform.position = ProgressionManager.Instance.LastVisitedLocation switch
        {
            ELastVisitedLocation.Island => islandSpawn.position,
            ELastVisitedLocation.City => citySpawn.position,
            ELastVisitedLocation.Moon => moonSpawn.position,
            _ => spaceStationSpawn.position,
        };
        yield return new WaitForSeconds(1f);
        GameManager.Instance.PauseMenuHandler.FadeTime = fadeBufferTime;

        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeOut(null));
    }
    #endregion

    #region Unity Stuff

    private void Awake()
    {
        controls = new();

        controls.SpaceShip.Accelerate.started += ctx => ToggleAcceleration(ctx);
        controls.SpaceShip.Accelerate.canceled += ctx => ToggleAcceleration(ctx);
        controls.SpaceShip.Rotate.performed += ctx => ReadShipDirection(ctx);
        controls.SpaceShip.Land.performed += ctx => Land(ctx);
        controls.SpaceShip.ExitOrbit.performed += ctx => ExitOrbit(ctx);
    }

    private void Start()
    {
        velocity = new Vector2(4, -3);
        accelerate = false;
        if (thrusterSource != null) maxThrusterVolume = thrusterSource.volume;
        shipTransform = transform;

        switch (ProgressionManager.Instance.LastVisitedLocation)
        {
            case ELastVisitedLocation.SpaceStation:
                shipTransform.position = spaceStationSpawn.position;
                break;
            case ELastVisitedLocation.Island:
                shipTransform.position = islandSpawn.position;
                break;
            case ELastVisitedLocation.City:
                shipTransform.position = citySpawn.position;
                break;
            case ELastVisitedLocation.Moon:
                shipTransform.position = moonSpawn.position;
                break;
            default: break;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentInputScheme == EInputScheme.MouseKeyboard)
        {
            Vector3 shipPos = cam.WorldToScreenPoint(shipTransform.position);
            lookDirection = new Vector3(shipInput.x, shipInput.y) - shipPos;
        }
        else lookDirection = shipInput;
        if (lookDirection != Vector2.zero)
            shipTransform.up = lookDirection;

        if (GameManager.Instance.IsPlaying && !GameManager.Instance.PauseMenuHandler.LoadingScreen.gameObject.activeInHierarchy && !GameManager.Instance.IsFading)
        {
            if (orbiting)
            {
                MoveInOrbit();
            }
            else if (emergencyMode)
            {
                shipTransform.position += new Vector3(velocity.x, velocity.y) * Time.deltaTime / 2f;
            }
            else
            {
                var force = gravityReceiver.CalculateForce() * Time.deltaTime;

                velocity += force;
                if (accelerate)
                {
                    velocity += accelerationForce * Time.deltaTime * new Vector2(shipTransform.up.x, shipTransform.up.y);
                }
                shipTransform.position += new Vector3(velocity.x, velocity.y) * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Orbit Trigger") && currentPlanet == null)
        {
            currentPlanet = collision.transform.parent.GetComponent<GravityObject>();
            InitiateOrbit(collision.transform.parent.position);
        }
        else if (collision.CompareTag("Planet Surface"))
        {
            emergencyMode = true;
        }
        else if (collision.CompareTag("Sun Trigger"))
        {
            fadeBufferTime = GameManager.Instance.PauseMenuHandler.FadeTime;
            GameManager.Instance.PauseMenuHandler.FadeTime = 2f;

            UnityEvent onFadeInFinished = new();
            onFadeInFinished.AddListener(() =>
            {
                StartCoroutine(RespawnShipAfterSunImpact());
            });
            StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(onFadeInFinished));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet Surface"))
        {
            emergencyMode = false;
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
