using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

/// <summary>This class is the character controller for Nova, who is controlled in Locations</summary>
public class NovaMovement : MonoBehaviour
{
    #region Fields
    /// <summary>Input System Class/// </summary>
    private SpacePiratesControls controls;

    /// <summary>Caches the movement input for physics calculation</summary>
    private Vector2 moveInput;

    /// <summary>The time it takes to charge a bouncy shot</summary>
    [SerializeField] private float chargeAttackTime;

    /// <summary>The max speed at which Nove moves</summary>
    [SerializeField] private float movementSpeed;

    [SerializeField] private InteractionPrompt interactionPrompt;
    [SerializeField] private float fallTime;
    private float fallTimer;
    /// <summary>Reference to Nova's rigidbody2D</summary>
    [Header("References"), SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform ballSpawnPosition;
    [SerializeField] private Camera mainCamera;
    private Transform bulletContainer;
    [SerializeField] private Canvas mainCanvas;

    [SerializeField] private GameObject wallCollider;
    [SerializeField] private GameObject damageCollider;
    [SerializeField] private GameObject buttonTrigger;
    
    [HideInInspector] public Rigidbody2D MovableObject;

    [Header("Prefabs"), SerializeField] private GameObject chargedBulletPrefab;
    [SerializeField] private GameObject smallBulletPrefab;
    [SerializeField] private GameObject interacttionPromptPrefab;

    [Header("Audio Assets"), SerializeField] private AudioSource chargeAudioSource;

    private float chargeAttackTimer;

    private Vector2 attackDirection;

    private List<InteractableTrigger> interactableTriggers;

    private InteractableTrigger performedInteraction;

    private float fallingSpeed;

    private bool doFall;

    private int sortingOffset;

    private BoxCollider2D firstFloorMovableBox;

    #endregion

    #region Methods
    /// <summary>Caches the movement input for Nova</summary>
    /// <param name="ctx"></param>
    private void ReadMovementInput(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.ChangeInputScheme(ctx);
        moveInput = ctx.ReadValue<Vector2>();
    }

    /// <summary>Does a ranged attack depending on how long the button has been pressed</summary>
    /// <param name="ctx"></param>
    private void DoRangedAttack(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.ChangeInputScheme(ctx);

        if (GameManager.Instance.IsPlaying)
        {
            if (ctx.action.WasReleasedThisFrame())
            {
                chargeAudioSource.Stop();

                if (attackDirection == Vector2.zero) attackDirection = Vector2.right;

                if (chargeAttackTimer <= 0) SpawnBullet(chargedBulletPrefab);
                else SpawnBullet(smallBulletPrefab);
            }
            else if (ctx.action.WasPerformedThisFrame())
            {
                chargeAttackTimer = chargeAttackTime;
                chargeAudioSource.PlayDelayed(.15f);
            }
        }
    }

    private void SpawnBullet(GameObject bulletPrefab)
    {
        var bulletGO = Instantiate(bulletPrefab);
        var bullet = bulletGO.GetComponent<ChargedBullet>();

        bulletGO.transform.position = ballSpawnPosition.position;
        bulletGO.layer = ballSpawnPosition.gameObject.layer;
        //chargedBulletGO.tag = tag;
        bullet.Rb.velocity = attackDirection.normalized * bullet.MovementSpeed;
        bullet.GetComponent<DamageSource>().Origin = DamageOriginator.Player;

        bulletGO.transform.parent = bulletContainer;
    }

    private void SetAttackDirection(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.ChangeInputScheme(ctx);

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

    private void DoInteract(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.ChangeInputScheme(ctx);

        if (GameManager.Instance.IsPlaying)
        {
            if (ctx.action.WasPerformedThisFrame())
            {
                if (performedInteraction != null)
                {
                    performedInteraction.Interact();
                    interactionPrompt.Hide();
                }
            }
            if (ctx.action.WasReleasedThisFrame())
            {
                if (performedInteraction != null)
                {
                    performedInteraction.StopInteract();

                    if (!interactableTriggers.Contains(performedInteraction))
                    {
                        performedInteraction = null;
                    }
                    else interactionPrompt.EnablePrompt(performedInteraction.InteractText, controls.Nova.Interact.bindings);
                }
            }
        }
    }

    public void SwitchFloor(bool groundFloor)
    {
        if (groundFloor) SetColliderLayers(3, 7, 8, 6);
        else SetColliderLayers(13, 10, 11, 12);
    }

    private void SetColliderLayers(int wall, int damage, int button, int bullets)
    {
        wallCollider.layer = wall;
        damageCollider.layer = damage;
        buttonTrigger.layer = button;
        ballSpawnPosition.gameObject.layer = bullets;
    }

    public void BeginFall()
    {
        if (rb.velocity.y > 0) sortingOffset = 0;
        doFall = true;
        // yPosAtBeginningOfFall = transform.position.y;
        fallingSpeed = 8f;
        fallTimer = fallTime;
    }

    private void StopFall()
    {
        sortingOffset = 0;
        fallTimer = 0;
        doFall = false;
        rb.velocity = new();
        if (firstFloorMovableBox != null)
        {
            firstFloorMovableBox.enabled = true;
            firstFloorMovableBox = null;
        }
    }
    #endregion

    #region Unity Stuff
    private void Awake()
    {
        controls ??= new();

        controls.Nova.Move.performed += ctx => ReadMovementInput(ctx);
        controls.Nova.RangedAttack.performed += ctx => DoRangedAttack(ctx);
        controls.Nova.RangedAttack.canceled += ctx => DoRangedAttack(ctx);

        controls.Nova.Aim.performed += ctx => SetAttackDirection(ctx);
        controls.Nova.Interact.performed += ctx => DoInteract(ctx);
        controls.Nova.Interact.canceled += ctx => DoInteract(ctx);

        interactableTriggers = new();

        if (bulletContainer == null)
        {
            var bulletsGO = GameObject.Find("Bullets");
            if (bulletsGO == null)
            {
                bulletsGO = new();
                bulletsGO.name = "Bullets";
                Debug.LogWarning("Player had to spawn the Bullets GameObject! Please make sure it exists and assign it to the bulletContainer field!");
            }
            bulletContainer = bulletsGO.transform;
        }
        GameManager.Instance.Nova = this;
        fallTimer = 0;
        sortingOffset = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            if (chargeAttackTimer >= 0) chargeAttackTimer -= Time.deltaTime;
            spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y) + sortingOffset;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsPlaying)
        {
            rb.velocity = movementSpeed * Time.fixedDeltaTime * moveInput;
            if (MovableObject != null) rb.velocity += MovableObject.velocity;

            if (doFall)
            {
                //fallingSpeed += Time.fixedDeltaTime;
                
                if (fallTimer <= 0)
                {
                    StopFall();
                    SwitchFloor(true);
                }
                else
                {
                    fallTimer -= Time.fixedDeltaTime;
                    fallingSpeed -= Time.fixedDeltaTime * 25f;
                    rb.velocity += new Vector2(0, fallingSpeed);
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out InteractableTrigger interactableTrigger))
        {
            interactableTriggers.Add(interactableTrigger);
            performedInteraction = interactableTriggers[0];

            //todo: show interact prompt
            if (interactionPrompt == null)
            {
                var go = Instantiate(interacttionPromptPrefab);
                interactionPrompt = go.GetComponent<InteractionPrompt>();
                interactionPrompt.transform.SetParent(mainCanvas.transform, false);
            }
            interactionPrompt.EnablePrompt(performedInteraction.InteractText, controls.Nova.Interact.bindings, performedInteraction.gameObject.transform);
        }
        else if (collision.gameObject.CompareTag("1st Floor") && fallTimer > fallTime / 3f)
        {
            StopFall();
            firstFloorMovableBox = collision.gameObject.GetComponent<BoxCollider2D>();
            sortingOffset = 3;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out InteractableTrigger interactableTrigger))
        {
            if (interactableTriggers.Contains(interactableTrigger))
            {
                interactableTriggers.Remove(interactableTrigger);
                if (interactableTrigger == performedInteraction)
                {
                    performedInteraction.StopInteract();
                }
            }
            interactionPrompt.Hide();
        }
        else if (collision.gameObject.CompareTag("1st Floor") && firstFloorMovableBox != null)
        {
            firstFloorMovableBox.enabled = false;
            BeginFall();
        }
    }
    /// <summary>This helps make sure that I set up the character in new scenes correctly lol</summary>
    private void OnValidate()
    {
        if (gameObject.scene.name != null && gameObject.scene.name != gameObject.name)
        {
            if (mainCanvas == null) Debug.LogWarning($"Main Canvas not assigned for {gameObject.name} in Scene {gameObject.scene.name}");
            if (mainCamera == null) Debug.LogWarning($"Main Camera not assigned for {gameObject.name} in Scene {gameObject.scene.name}");
        }
    }
    #endregion
}
