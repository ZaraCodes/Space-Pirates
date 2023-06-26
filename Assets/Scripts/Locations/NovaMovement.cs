using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NovaMovement : MonoBehaviour
{
    private SpacePiratesControls controls;

    private Vector2 moveInput;

    [SerializeField] private float movementSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera mainCamera;


    private void ReadMovementInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }


    #region Unity Stuff
    private void Awake()
    {
        controls ??= new();

        controls.Nova.Move.performed += ctx => ReadMovementInput(ctx);
    }

    private void FixedUpdate()
    {
        if (true)
        {
            rb.velocity = moveInput * Time.fixedDeltaTime * movementSpeed;
            mainCamera.transform.position = new(transform.position.x, transform.position.y, mainCamera.transform.position.z);
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
