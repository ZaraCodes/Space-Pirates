using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

public class HopeShip : MonoBehaviour
{
    private SpacePiratesControls controls;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera cam;
    [SerializeField] private GravityReceiver gravityReceiver;
    [SerializeField] private AudioSource thrusterSource;

    private Coroutine thrusterFadeCoroutine;

    private float maxThrusterVolume;

    private Vector2 lookDirection;

    private Vector2 shipInput;

    private Vector2 velocity;

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

    private void ReadShipDirection(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (GameManager.Instance.IsPlaying)
        {
            shipInput = ctx.ReadValue<Vector2>();
        }
    }

    public void MoveInOrbit()
    {
        Vector3 oldPos = shipTransform.position;
        orbitTime += Time.deltaTime / currentPlanet.OrbitSpeed * orbitDirection;
        shipTransform.position = OrbitCenter + new Vector3(Mathf.Sin(orbitTime), Mathf.Cos(orbitTime)) * orbitDistance;

        velocity = -(oldPos - shipTransform.position) / Time.deltaTime;
    }

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

        Vector3 directionA = minDistanceToShip - OrbitCenter + new Vector3(Mathf.Sin(orbitTime + Time.fixedDeltaTime), Mathf.Cos(orbitTime + Time.fixedDeltaTime)) * orbitDistance;
        Vector3 directionB = minDistanceToShip - OrbitCenter + new Vector3(Mathf.Sin(orbitTime - Time.fixedDeltaTime), Mathf.Cos(orbitTime - Time.fixedDeltaTime)) * orbitDistance;

        if (Vector3.Angle(velocity, directionA) < Vector3.Angle(velocity, directionB)) orbitDirection = 1;
        else orbitDirection = -1;

        rb.velocity = Vector2.zero;
        OrbitCenter = center;
        orbiting = true;
        ShowPlanetPrompts();
    }

    public void ShowPlanetPrompts()
    {
        landPrompt.EnablePrompt(landText, controls.SpaceShip.Land.bindings);
        exitOrbitPrompt.EnablePrompt(exitOrbitText, controls.SpaceShip.ExitOrbit.bindings);
    }

    public void HidePlanetPrompts()
    {
        landPrompt.Hide();
        exitOrbitPrompt.Hide();
    }

    public void Land(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);
        if (ctx.action.WasPerformedThisFrame() && currentPlanet != null)
        {
            SceneManager.LoadScene(currentPlanet.SceneIndexToLoad);
        }
    }

    public void ExitOrbit(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (ctx.action.WasPerformedThisFrame())
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
        maxThrusterVolume = thrusterSource.volume;
        shipTransform = transform;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentInputScheme == EInputScheme.MouseKeyboard)
        {
            Vector3 shipPos = cam.WorldToScreenPoint(shipTransform.position);
            lookDirection = new Vector3(shipInput.x, shipInput.y) - shipPos;
        }
        if (lookDirection != Vector2.zero)
            shipTransform.up = lookDirection;

        if (orbiting)
        {
            MoveInOrbit();
        }
        else
        {
            var force = gravityReceiver.CalculateForce() * Time.deltaTime;

            velocity += force;
            if (accelerate)
            {                
                velocity += new Vector2(shipTransform.up.x, shipTransform.up.y) * Time.deltaTime;
            }
            shipTransform.position += new Vector3(velocity.x, velocity.y) * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Orbit Trigger") && currentPlanet == null)
        {
            currentPlanet = collision.transform.parent.GetComponent<GravityObject>();
            InitiateOrbit(collision.transform.parent.position);
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
