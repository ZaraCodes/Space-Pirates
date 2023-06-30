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
    [SerializeField] private Transform ballSpawnPosition;
    [SerializeField] private Camera mainCamera;

    [Header("Prefabs")]
    [SerializeField] private GameObject chargedBulletPrefab;
    [SerializeField] private GameObject smallBulletPrefab;

    [Header("Audio Assets")]
    [SerializeField] private AudioSource chargeAudioSource;



    private float chargeAttackTimer;

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
            StopAllCoroutines();
            chargeAudioSource.Stop();

            if (attackDirection == Vector2.zero) attackDirection = Vector2.right;

            if (chargeAttackTimer <= 0)
            {
                var chargedBulletGO = Instantiate(chargedBulletPrefab);
                var chargedBullet = chargedBulletGO.GetComponent<ChargedBullet>();

                chargedBulletGO.transform.position = ballSpawnPosition.position;
                chargedBulletGO.tag = tag;
                chargedBullet.Rb.velocity = attackDirection.normalized * chargedBullet.MovementSpeed;
            }
            else
            {
                var smallBulletGO = Instantiate(smallBulletPrefab);
                var smallBullet = smallBulletGO.GetComponent<ChargedBullet>();

                smallBulletGO.transform.position = ballSpawnPosition.position;
                smallBulletGO.tag = tag;
                smallBullet.Rb.velocity = attackDirection.normalized * smallBullet.MovementSpeed;
            }
        }
        else if (ctx.action.WasPerformedThisFrame())
        {
            chargeAttackTimer = chargeAttackTime;
            StartCoroutine(StartChargeAudioDelayed());
        }
    }

    private IEnumerator StartChargeAudioDelayed()
    {
        yield return new WaitForSeconds(0.15f);
        chargeAudioSource.Play();
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
        if (chargeAttackTimer >= 0 && GameManager.Instance.IsPlaying) chargeAttackTimer -= Time.deltaTime;
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
