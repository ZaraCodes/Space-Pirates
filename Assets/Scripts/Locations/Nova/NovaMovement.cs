using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NovaMovement : MonoBehaviour
{
    private SpacePiratesControls controls;

    private Vector2 moveInput;


    [SerializeField] private float chargeAttackTime;
    [SerializeField] private float movementSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject chargedBulletPrefab;


    private float startTimeRangedAttackInput;
    private float stopTimeRangedAttackInput;

    private Vector2 attackDirection;

    /// <summary>Handles the movement input for Nova</summary>
    /// <param name="ctx"></param>
    private void ReadMovementInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void DoRangedAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasReleasedThisFrame())
        {
            stopTimeRangedAttackInput = Time.time;
            var inputTime = stopTimeRangedAttackInput - startTimeRangedAttackInput;

            if (inputTime > chargeAttackTime)
            {
                var chargedBulletGO = Instantiate(chargedBulletPrefab);
                var chargedBullet = chargedBulletGO.GetComponent<ChargedBullet>();

                chargedBulletGO.transform.position = transform.position;
                chargedBullet.Rb.velocity = attackDirection.normalized * chargedBullet.MovementSpeed;
            }
            else
            {
                //Todo: Spawn normal bullet
            }
        }
        else if (ctx.action.WasPerformedThisFrame())
        {
            startTimeRangedAttackInput = Time.time;
        }
    }

    private void SetAttackDirection(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.IsPlaying)
        {
            Vector2 direction = ctx.ReadValue<Vector2>();
            if (ctx.control.device.displayName.Contains("Mouse"))
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
                direction = new Vector3(direction.x, direction.y) - screenPos;

            }
            if (direction != Vector2.zero)
                attackDirection = direction;
        }
    }


    #region Unity Stuff
    private void Awake()
    {
        controls ??= new();

        controls.Nova.Move.performed += ctx => ReadMovementInput(ctx);
        controls.Nova.RangedAttack.performed += ctx => DoRangedAttack(ctx);
        controls.Nova.RangedAttack.canceled += ctx => DoRangedAttack(ctx);

        controls.SpaceShip.Rotate.performed += ctx => SetAttackDirection(ctx);
    }

    private void FixedUpdate()
    {
        if (true)
        {
            rb.velocity = moveInput * Time.fixedDeltaTime * movementSpeed;
        }
    }

    private void LateUpdate()
    {
        mainCamera.transform.position = new(transform.position.x, transform.position.y, mainCamera.transform.position.z);
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
