using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

/// <summary>This class is the character controller for Nova, who is controlled in Locations</summary>
public class NovaMovement : MonoBehaviour
{
    #region Fields
    /// <summary>Input System Class/// </summary>
    private SpacePiratesControls controls;

    /// <summary>Caches the movement input for physics calculation</summary>
    public Vector2 MoveInput { get; set; }

    /// <summary>The time it takes to charge a bouncy shot</summary>
    [SerializeField] private float chargeAttackTime;

    /// <summary>The max speed at which Nove moves</summary>
    [SerializeField] private float movementSpeed;

    /// <summary>The max speed at which Nove moves</summary>
    public float MovementSpeed { get { return movementSpeed; } }

    [SerializeField] private InteractionPrompt interactionPrompt;
    [SerializeField] private float fallTime;
    public float FallTime { get { return fallTime; } }
    private float fallTimer;
    public float FallTimer { get { return fallTimer; } }

    private bool switchFloor;

    private Vector2 jumpDirection;
    public FloorTransition TransitionTrigger { get; set; }

    public Vector2 MovementConstraint { get; set; }

    public bool ZeroGMovement { get; set; }

    public bool CutsceneMovement { get; set; }

    public Vector3 RespawnPosition { get; set; }

    /// <summary>Reference to Nova's rigidbody2D</summary>
    [Header("References"), SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer animationSprites;
    [SerializeField] private Animator animator;
    public Animator Animator { get { return animator; } }
    [SerializeField] private Transform ballSpawnPosition;
    [SerializeField] private Camera mainCamera;
    private Transform bulletContainer;
    [SerializeField] private Canvas mainCanvas;

    [SerializeField] private GameObject wallCollider;
    [SerializeField] private GameObject damageCollider;
    [SerializeField] private GameObject buttonTrigger;

    [HideInInspector] public Rigidbody2D MovableObject;

    [SerializeField] private CollisionManager collisionManager;

    [SerializeField] private Tilemap groundTilemap;

    [Header("Prefabs"), SerializeField] private GameObject chargedBulletPrefab;
    [SerializeField] private GameObject smallBulletPrefab;
    [SerializeField] private GameObject interacttionPromptPrefab;

    [Header("Audio Assets"), SerializeField] private AudioSource chargeAudioSource;

    private float chargeAttackTimer;

    private Vector2 attackDirection;

    private Vector2 initialAttackDirectionInput;

    private List<InteractableTrigger> interactableTriggers;

    private InteractableTrigger performedInteraction;

    private float fallingSpeed;

    public bool DoFall { get; set; }

    private int sortingOffset;

    private BoxCollider2D firstFloorMovableBox;

    public bool Fading;

    public Vector2 RbVelBuffer { get; set; }
    #endregion

    #region Methods
    /// <summary>Caches the movement input for Nova</summary>
    /// <param name="ctx"></param>
    private void ReadMovementInput(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (!ZeroGMovement)
            MoveInput = ctx.ReadValue<Vector2>() * MovementConstraint;
    }

    /// <summary>Does a ranged attack depending on how long the button has been pressed</summary>
    /// <param name="ctx"></param>
    private void DoRangedAttack(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (GameManager.Instance.IsPlaying)
        {
            if (ctx.action.WasReleasedThisFrame())
            {
                animator.SetTrigger("shooting");
                chargeAudioSource.Stop();

                if (attackDirection == Vector2.zero) attackDirection = Vector2.right;

                if (chargeAttackTimer <= 0) SpawnBullet(chargedBulletPrefab);
                else SpawnBullet(smallBulletPrefab);
            }
            else if (ctx.action.WasPerformedThisFrame())
            {
                animator.SetTrigger("charging");
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

        bullet.Rb.velocity = attackDirection.normalized * bullet.MovementSpeed;
        bullet.GetComponent<DamageSource>().Origin = DamageOriginator.Player;

        bulletGO.transform.parent = bulletContainer;

        if (ZeroGMovement) MoveInput = attackDirection.normalized;
    }

    private void SetAttackDirection(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (GameManager.Instance.IsPlaying)
        {
            initialAttackDirectionInput = ctx.ReadValue<Vector2>();
            if (initialAttackDirectionInput == Vector2.zero) initialAttackDirectionInput = Vector2.down;
            if (GameManager.Instance.CurrentInputScheme == EInputScheme.Gamepad) attackDirection = initialAttackDirectionInput;
        }
    }

    private void DoInteract(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

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
        if (groundFloor)
        {
            if (!IsOnGround())
            {
                Respawn();
            }
            else
            {
                spriteRenderer.color = Color.white;
                SetColliderLayers(3, 7, 8, 6);
            }
        }
        else
        {
            spriteRenderer.color = Color.gray;
            SetColliderLayers(13, 10, 11, 12);
        }
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
        DoFall = true;
        // yPosAtBeginningOfFall = transform.position.y;
        fallingSpeed = 8f;
        fallTimer = fallTime;
        jumpDirection = MoveInput;

        if (collisionManager != null) collisionManager.DisableCollisions();
    }

    public void StopFall(bool topLanding = false)
    {
        if (topLanding)
        {
            switchFloor = false;
            var newFallTimer = fallTimer - fallTime / 4f;
            fallTimer = newFallTimer > 0 ? newFallTimer : fallTimer;
            //Debug.Log(fallTimer);
        }
        else
        {
            if (collisionManager != null) collisionManager.EnableCollisions();

            if (TransitionTrigger != null)
            {
                TransitionTrigger.TriggerEnabled = true;
                TransitionTrigger = null;
            }
            sortingOffset = 0;
            fallTimer = 0;
            DoFall = false;
            rb.velocity = new();
            if (firstFloorMovableBox != null)
            {
                firstFloorMovableBox.enabled = true;
                firstFloorMovableBox = null;
            }
        }
    }

    public void ToggleZeroG(bool zeroG)
    {
        ZeroGMovement = zeroG;
    }

    public bool IsOnGround()
    {
        if (groundTilemap != null && !groundTilemap.HasTile(new Vector3Int((int)transform.position.x, (int)transform.position.y))) return false;
        else return true;
    }

    public void Respawn()
    {
        transform.position = RespawnPosition;
    }

    public void ShowInteractionPrompt(InteractableTrigger interactableTrigger)
    {
        //if (collision.gameObject.TryGetComponent(out InteractableTrigger interactableTrigger))
        //{

        //print((collision.transform.position - transform.position).magnitude);
        //if ((collision.transform.position - transform.position).magnitude > 3f) return;

        if (!interactableTriggers.Contains(interactableTrigger))
        {
            interactableTriggers.Add(interactableTrigger);
            performedInteraction = interactableTriggers[0];
            //performedInteraction = interactableTrigger;

            if (interactionPrompt == null)
            {
                var go = Instantiate(interacttionPromptPrefab);
                interactionPrompt = go.GetComponent<InteractionPrompt>();
                interactionPrompt.transform.SetParent(mainCanvas.transform, false);
                interactionPrompt.transform.SetAsFirstSibling();
            }
            interactionPrompt.EnablePrompt(performedInteraction.InteractText, controls.Nova.Interact.bindings, performedInteraction.gameObject.transform);
        }
        //}
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
            if (bulletsGO.GetComponent<BulletContainer>() == null) bulletsGO.AddComponent<BulletContainer>();
            bulletContainer = bulletsGO.transform;
        }
        GameManager.Instance.Nova = this;
        fallTimer = 0;
        sortingOffset = 0;
        switchFloor = true;

        Fading = false;
        MovementConstraint = new(1, 1);
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            if (chargeAttackTimer >= 0)
            {
                chargeAttackTimer -= Time.deltaTime;
                animator.SetFloat("chargeTimer", chargeAttackTimer);
            }
            spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y) + sortingOffset - 1;
            animationSprites.sortingOrder = -Mathf.RoundToInt(transform.position.y) + sortingOffset;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsPlaying)
        {
            if (GameManager.Instance.CurrentInputScheme == EInputScheme.MouseKeyboard)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
                attackDirection = (new Vector3(initialAttackDirectionInput.x, initialAttackDirectionInput.y) - screenPos).normalized;
            }

            animator.SetFloat("attackDirX", attackDirection.x);
            animator.SetFloat("attackDirY", attackDirection.y);

            if (ZeroGMovement)
            {
                rb.velocity *= .99f;
                if (MoveInput != Vector2.zero)
                {
                    rb.AddForce(-MoveInput);
                    MoveInput = Vector2.zero;
                }
            }
            else if (!CutsceneMovement && !Fading)
            {
                rb.velocity = movementSpeed * Time.fixedDeltaTime * (DoFall ? jumpDirection * 0.8f : MoveInput);

                var normalizedVelocity = rb.velocity.normalized;
                animator.SetFloat("velocityX", normalizedVelocity.x);
                animator.SetFloat("velocityY", normalizedVelocity.y);
            }

            if (MovableObject != null)
            {
                rb.velocity += MovableObject.velocity;

                var normalizedVelocity = rb.velocity.normalized - MovableObject.velocity;
                animator.SetFloat("velocityX", normalizedVelocity.x);
                animator.SetFloat("velocityY", normalizedVelocity.y);
            }

            if (DoFall)
            {
                if (fallTimer <= 0)
                {
                    if (collisionManager != null) collisionManager.EnableCollisions();

                    StopFall();
                    if (switchFloor) SwitchFloor(true);
                    else switchFloor = true;
                }
                else
                {
                    fallTimer -= Time.fixedDeltaTime;
                    fallingSpeed -= Time.fixedDeltaTime * 25f;
                    rb.velocity += new Vector2(0, fallingSpeed);
                }
            }
            else if (MovableObject == null)
            {
                RespawnPosition = transform.position;
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
        // movable box 1st floor
        if (collision.gameObject.CompareTag("1st Floor") && fallTimer > fallTime / 4f)
        {
            StopFall();
            firstFloorMovableBox = collision.gameObject.GetComponent<BoxCollider2D>();
            MovableObject = firstFloorMovableBox.GetComponentInParent<Rigidbody2D>();

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
                    performedInteraction = null;
                }
            }
            interactionPrompt.Hide();
        }
        else if (collision.gameObject.CompareTag("1st Floor") && firstFloorMovableBox != null)
        {
            firstFloorMovableBox.enabled = false;
            MovableObject = null;
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
