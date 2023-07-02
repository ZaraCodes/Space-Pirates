using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>This class is the character controller for Nova, who is controlled in Locations</summary>
public class NovaMovement : MonoBehaviour
{
    /// <summary>Input System Class/// </summary>
    private SpacePiratesControls controls;

    /// <summary>Caches the movement input for physics calculation</summary>
    private Vector2 moveInput;

    /// <summary>The time it takes to charge a bouncy shot</summary>
    [SerializeField] private float chargeAttackTime;

    /// <summary>The max speed at which Nove moves</summary>
    [SerializeField] private float movementSpeed;


    /// <summary>Reference to Nova's rigidbody2D</summary>
    [Header("References"), SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform ballSpawnPosition;
    [SerializeField] private Camera mainCamera;

    [Header("Prefabs"), SerializeField] private GameObject chargedBulletPrefab;
    [SerializeField] private GameObject smallBulletPrefab;

    [Header("Audio Assets"), SerializeField] private AudioSource chargeAudioSource;



    private float chargeAttackTimer;

    private Vector2 attackDirection;

    /// <summary>Caches the movement input for Nova</summary>
    /// <param name="ctx"></param>
    private void ReadMovementInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void DoRangedAttack(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.IsPlaying)
        {
            if (ctx.action.WasReleasedThisFrame())
            {
                chargeAudioSource.Stop();

                if (attackDirection == Vector2.zero) attackDirection = Vector2.right;

                if (chargeAttackTimer <= 0)
                {
                    var chargedBulletGO = Instantiate(chargedBulletPrefab);
                    var chargedBullet = chargedBulletGO.GetComponent<ChargedBullet>();

                    chargedBulletGO.transform.position = ballSpawnPosition.position;
                    chargedBulletGO.tag = tag;
                    chargedBullet.Rb.velocity = attackDirection.normalized * chargedBullet.MovementSpeed;
                    chargedBullet.GetComponent<DamageSource>().Origin = DamageOriginator.Player;
                }
                else
                {
                    var smallBulletGO = Instantiate(smallBulletPrefab);
                    var smallBullet = smallBulletGO.GetComponent<ChargedBullet>();

                    smallBulletGO.transform.position = ballSpawnPosition.position;
                    smallBulletGO.tag = tag;
                    smallBullet.Rb.velocity = attackDirection.normalized * smallBullet.MovementSpeed;
                    smallBullet.GetComponent<DamageSource>().Origin = DamageOriginator.Player;
                }
            }
            else if (ctx.action.WasPerformedThisFrame())
            {
                chargeAttackTimer = chargeAttackTime;
                chargeAudioSource.PlayDelayed(.15f);
            }
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

        controls.Nova.Aim.performed += ctx => SetAttackDirection(ctx);
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            if (chargeAttackTimer >= 0) chargeAttackTimer -= Time.deltaTime;
            spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y);
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsPlaying)
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
