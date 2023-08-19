using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

/// <summary>
/// The space ship to fly around in space
/// </summary>
public class HopeShip : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the controls</summary>
    private SpacePiratesControls controls;

    /// <summary>The force with which the ship accelerates</summary>
    [SerializeField] private float accelerationForce;

    /// <summary>Reference to the ship rigidbody</summary>
    [SerializeField] private Rigidbody2D rb;
    /// <summary>Reference to the camera</summary>
    [SerializeField] private Camera cam;
    /// <summary>Reference to the ship's gravity receiver</summary>
    [SerializeField] private GravityReceiver gravityReceiver;
    /// <summary>Reference to the thruster audio source</summary>
    [SerializeField] private AudioSource thrusterSource;
    /// <summary>if true, the ship cannot be controlled bc it's in the intro cutscene</summary>
    [SerializeField] private bool introShip;

    /// <summary>Cache of the thruster audio fade coroutine</summary>
    private Coroutine thrusterFadeCoroutine;
    /// <summary>the max volume of the thruster audio</summary>
    private float maxThrusterVolume;
    /// <summary>The direction the ship faces in</summary>
    private Vector2 lookDirection;
    /// <summary>The input the ship receives from the game</summary>
    private Vector2 shipInput;
    /// <summary>The ship's current velocity</summary>
    private Vector2 velocity;
    /// <summary>The ship's current velocity</summary>
    public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
    /// <summary>Does the ship currently accelerate</summary>
    private bool accelerate;
    /// <summary>The transform component of the ship</summary>
    private Transform shipTransform;
    /// <summary>The time the ship currently is orbit of a planet</summary>
    private float orbitTime;
    /// <summary>Does the ship currently orbit around a planet</summary>
    private bool orbiting;
    /// <summary>Reference to the planet the ship orbits around</summary>
    private GravityObject currentPlanet;
    /// <summary>The center point of the orbit</summary>
    public Vector3 OrbitCenter { get; private set; }
    /// <summary>The distance of orbit</summary>
    private float orbitDistance;
    /// <summary>The direction the ship will orbit in (1, -1)</summary>
    private float orbitDirection;

    /// <summary>The particle system that produces the ship thruster particles</summary>
    [SerializeField] private ParticleSystem thrusterParticles;

    /// <summary>Reference to the prompt that shows the controls to land</summary>
    [SerializeField] private InteractionPrompt landPrompt;
    /// <summary>Reference to the prompt that shows the controls to exit the orbit</summary>
    [SerializeField] private InteractionPrompt exitOrbitPrompt;
    /// <summary>The text to display for the landing</summary>
    [SerializeField] private LocalizedString landText;
    /// <summary>The text to display for exiting the orbit</summary>
    [SerializeField] private LocalizedString exitOrbitText;

    /// <summary>Reference to the space station transform</summary>
    [SerializeField] private Transform spaceStationSpawn;
    /// <summary>Reference to the island planet transform</summary>
    [SerializeField] private Transform islandSpawn;
    /// <summary>Reference to the moon transform</summary>
    [SerializeField] private Transform moonSpawn;
    /// <summary>Reference to the city planet spawn</summary>
    [SerializeField] private Transform citySpawn;

    /// <summary>Emergency mode is when the ship flies into a planet. It then phases through the planet without accelerating while being inside of the planet and then continues normally on the other side</summary>
    private bool emergencyMode;
    /// <summaryBuffer of the fade timer </summary>
    private float fadeBufferTime;
    #endregion

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
    /// <param name="ctx">The callback context of the inout action</param>
    public void Land(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);
        if (ctx.action.WasPerformedThisFrame() && currentPlanet != null && !GameManager.Instance.IsFading)
        {
            // If both hints have been found, the player is only allowed to land on the moon
            if ((ProgressionManager.Instance.Flags.Contains(EProgressionFlag.IslandHint) &&
                ProgressionManager.Instance.Flags.Contains(EProgressionFlag.ShipSellerHint) &&
                currentPlanet.Location == ELastVisitedLocation.Moon)
                ||
                (!ProgressionManager.Instance.Flags.Contains(EProgressionFlag.IslandHint)))
            {
                ProgressionManager.Instance.LastVisitedLocation = currentPlanet.Location;

                UnityEvent unityEvent = new();
                unityEvent.AddListener(() => StartCoroutine(LoadPlanetAsync()));

                StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(unityEvent));
            }
        }
    }

    /// <summary>
    /// Coroutine that loads a planet async
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
    /// <param name="ctx">The callback context of the inout action</param>
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

    /// <summary>
    /// Coroutine spawns the ship at the last visited location if the player decides to fly into the sun for some reason
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Initiates the input system
    /// </summary>
    private void Awake()
    {
        controls = new();

        controls.SpaceShip.Accelerate.started += ctx => ToggleAcceleration(ctx);
        controls.SpaceShip.Accelerate.canceled += ctx => ToggleAcceleration(ctx);
        controls.SpaceShip.Rotate.performed += ctx => ReadShipDirection(ctx);
        controls.SpaceShip.Land.performed += ctx => Land(ctx);
        controls.SpaceShip.ExitOrbit.performed += ctx => ExitOrbit(ctx);
    }

    /// <summary>
    /// Spawns the ship at the last visited location
    /// </summary>
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
            default:
                shipTransform.position = spaceStationSpawn.position;
                break;
        }
    }

    /// <summary>
    /// Moves the ship in space, rotates the ship
    /// </summary>
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

    /// <summary>
    /// Makes the ship go into orbit mode, start the emergency mode to fly through a planet, or respawns the ship if it flies into the sun
    /// </summary>
    /// <param name="collision">The collider of the other object</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Orbit Trigger") && currentPlanet == null)
        {
            currentPlanet = collision.transform.parent.parent.GetComponent<GravityObject>();
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

    /// <summary>
    /// Exits the emergency mode if the ship exists a planet trigger
    /// </summary>
    /// <param name="collision">The collider of the other object</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet Surface"))
        {
            emergencyMode = false;
        }
    }

    /// <summary>Enables the controls system</summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>Disables the controls system</summary>
    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
