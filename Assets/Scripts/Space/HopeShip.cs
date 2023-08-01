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

    private Transform cameraTransform;

    private Vector2 velocity;

    private bool accelerate;

    private Transform shipTransform;

    public float OrbitTime;

    public bool Orbiting;

    private GravityObject currentPlanet;

    public Vector3 OrbitCenter { get; set; }

    public float OrbitDistance;

    [SerializeField] private InteractionPrompt landPrompt;
    [SerializeField] private InteractionPrompt exitOrbitPrompt;
    [SerializeField] private LocalizedString landText;
    [SerializeField] private LocalizedString exitOrbitText;

    #region vPMethods

    /// <summary>This method toggles the acceleration of the ship depending on the input</summary>
    /// <param name="ctx"></param>
    private void ToggleAcceleration(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (ctx.action.WasPressedThisFrame())
        {
            accelerate = true;
        }
        else if (ctx.action.WasReleasedThisFrame())
        {
            accelerate = false;
        }
    }

    private void RotateShip(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (GameManager.Instance.IsPlaying)
        {
            Vector2 direction = ctx.ReadValue<Vector2>();
            if (ctx.control.device.displayName.Contains("Mouse"))
            {
                Vector3 screenPos = cam.WorldToScreenPoint(shipTransform.position);
                direction = new Vector3(direction.x, direction.y) - screenPos;

            }
            if (direction != Vector2.zero)
                shipTransform.up = direction;
        }
    }

    public void MoveInOrbit()
    {
        Vector3 oldPos = shipTransform.position;
        OrbitTime += Time.fixedDeltaTime / 2;
        shipTransform.position = OrbitCenter + new Vector3(Mathf.Sin(OrbitTime), Mathf.Cos(OrbitTime)) * OrbitDistance;

        velocity = -(oldPos - shipTransform.position) / Time.fixedDeltaTime;

        //Vector2 p = new Vector2(OrbitCenter.x, OrbitCenter.y);
        //Vector2 s = new Vector2(shipTransform.position.x, shipTransform.position.y);

        //Vector2 d = s - p;
        //Vector2 qL = new Vector2(d.y, -d.x).normalized;
        //Vector2 vP = Vector2.Dot(velocity, qL) * qL;

        //Vector2 a = -(Vector2.Dot(vP, vP) / Mathf.Sqrt(d.magnitude)) * d.normalized;
        //velocity += a;
        //rb.velocity = velocity;

        //Debug.Log($"p-s:{p - s} d:{d} qL:{qL} vP:{vP} a:{a}");
    }

    public void InitiateOrbit(Vector3 center)
    {
        OrbitCenter = center;
        rb.velocity = Vector2.zero;

        OrbitDistance = currentPlanet.OrbitDistance;

        Vector3 minDistanceToShip = OrbitCenter + new Vector3(Mathf.Sin(0), Mathf.Cos(0)) * OrbitDistance;
        Vector3 shipPos = new(shipTransform.position.x, shipTransform.position.y);

        for (float i = 0; i < Mathf.PI * 2; i += Time.fixedDeltaTime)
        {
            Vector3 newMinDistanceToShip = OrbitCenter + new Vector3(Mathf.Sin(i), Mathf.Cos(i)) * OrbitDistance;
            if ((newMinDistanceToShip - shipPos).magnitude < (minDistanceToShip - shipPos).magnitude)
            {
                minDistanceToShip = newMinDistanceToShip;
                OrbitTime = i;
            }
        }
        shipTransform.position = minDistanceToShip;

        OrbitCenter = center;
        Orbiting = true;
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
        SceneManager.LoadScene(currentPlanet.SceneIndexToLoad);
    }

    public void ExitOrbit(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (ctx.action.WasPerformedThisFrame())
        {
            Orbiting = false;
            HidePlanetPrompts();
            currentPlanet = null;
        }
    }

    #endregion

    #region Unity Stuff

    private void Awake()
    {
        controls = new();

        controls.SpaceShip.Accelerate.started += ctx => ToggleAcceleration(ctx);
        controls.SpaceShip.Accelerate.canceled += ctx => ToggleAcceleration(ctx);
        controls.SpaceShip.Rotate.performed += ctx => RotateShip(ctx);
        controls.SpaceShip.Land.performed += ctx => Land(ctx);
        controls.SpaceShip.ExitOrbit.performed += ctx => ExitOrbit(ctx);

        cameraTransform = cam.transform;
    }

    private void Start()
    {
        velocity = new Vector2(4, -3);
        accelerate = false;
        shipTransform = transform;
    }

    private void FixedUpdate()
    {
        if (Orbiting)
        {
            MoveInOrbit();
        }
        else
        {
            var force = gravityReceiver.CalculateForce() * Time.fixedDeltaTime;

            velocity += force;
            if (accelerate)
            {
                velocity += new Vector2(shipTransform.up.x, shipTransform.up.y) * Time.fixedDeltaTime;
            }
            rb.velocity = velocity;
        }
    }

    private void LateUpdate()
    {
        // Sets the camera position at the position of the ship
        cameraTransform.position = new Vector3(shipTransform.position.x, shipTransform.position.y, cameraTransform.position.z);
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
