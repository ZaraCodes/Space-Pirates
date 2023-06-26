using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;

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

    #region Methods

    /// <summary>This method toggles the acceleration of the ship depending on the input</summary>
    /// <param name="ctx"></param>
    private void ToggleAcceleration(InputAction.CallbackContext ctx)
    {
        //print(callbackContext.control.device.displayName);
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

    #endregion

    #region Unity Stuff

    private void Awake()
    {
        controls = new();

        controls.SpaceShip.Accelerate.started += ctx => ToggleAcceleration(ctx);
        controls.SpaceShip.Accelerate.canceled += ctx => ToggleAcceleration(ctx);
        controls.SpaceShip.Rotate.performed += ctx => RotateShip(ctx);

        cameraTransform = cam.transform;
    }

    private void Start()
    {
        velocity = new Vector2(4, -4);
        accelerate = false;
        shipTransform = transform;
    }

    private void FixedUpdate()
    {
        var force = gravityReceiver.CalculateForce() * Time.fixedDeltaTime;

        velocity += force;
        if (accelerate)
        {
            velocity += new Vector2(shipTransform.up.x, shipTransform.up.y) * Time.fixedDeltaTime;
        }
        rb.velocity = velocity;
    }

    private void LateUpdate()
    {
        // Sets the camera position at the position of the ship
        cameraTransform.position = new Vector3(shipTransform.position.x, shipTransform.position.y, cameraTransform.position.z);
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
