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
    /// <summary>Readonly property to read the controls variable of nova</summary>
    public SpacePiratesControls Controls { get { return controls; } }
    /// <summary>Caches the movement input for physics calculation</summary>
    public Vector2 MoveInput { get; set; }

    /// <summary>The time it takes to charge a bouncy shot</summary>
    [SerializeField] private float chargeAttackTime;

    /// <summary>The max speed at which Nove moves</summary>
    [SerializeField] private float movementSpeed;
    /// <summary>The max speed at which Nove moves</summary>
    public float MovementSpeed { get { return movementSpeed; } }

    /// <summary>Cache of the interaction prompt that is used for interactable objects</summary>
    private InteractionPrompt interactionPrompt;
    /// <summary>Cache of the interaction prompt that is used for interactable objects</summary>
    public InteractionPrompt InteractionPrompt { get {  return interactionPrompt; } }
    /// <summary>The time it takes for Nova to reach the lower floor from a jump</summary>
    [SerializeField] private float fallTime;
    /// <summary>The time it takes for Nova to reach the lower floor from a jump</summary>
    public float FallTime { get { return fallTime; } }
    /// <summary>Timer that counts down from the fall time during a fall</summary>
    private float fallTimer;
    /// <summary>Timer that counts down from the fall time during a fall</summary>
    public float FallTimer { get { return fallTimer; } }
    /// <summary>if true, nova will switch floors</summary>
    private bool switchFloor;
    /// <summary>The direction nova will move in during a jump</summary>
    private Vector2 jumpDirection;
    /// <summary>The last floor transition trigger Nova used</summary>
    public FloorTransition TransitionTrigger { get; set; }
    /// <summary>Influences Nova's movement direction. This is primarily used for limiting their movement while interacting with a box</summary>
    public Vector2 MovementConstraint { get; set; }
    /// <summary>Indicates if Nove is currently under the influence of 0G. If true, they will only be able to move by firing the weapon</summary>
    public bool ZeroGMovement { get; set; }
    /// <summary>Indicates if Nova currently gets moved by a cutscene</summary>
    public bool CutsceneMovement { get; set; }
    /// <summary>The respawn position in case Nova is in an invalid position (only used in the island scene)</summary>
    public Vector3 RespawnPosition { get; set; }

    /// <summary>Reference to Nova's rigidbody2D</summary>
    [Header("References"), SerializeField] private Rigidbody2D rb;
    /// <summary>Reference to Nova's Sprite Renderer</summary>
    [SerializeField] private SpriteRenderer animationSprites;
    /// <summary>Reference to Nova's Animator</summary>
    [SerializeField] private Animator animator;
    /// <summary>Grants reading access to Nova's Animator</summary>
    public Animator Animator { get { return animator; } }
    /// <summary>Transform for spawning bullets</summary>
    [SerializeField] private Transform ballSpawnPosition;
    /// <summary>Reference to the main camera</summary>
    [SerializeField] private Camera mainCamera;
    /// <summary>Reference to the bullet container of the scene. When Nova goes through a door, all existing bullets that are a child of this object get deleted</summary>
    private Transform bulletContainer;
    /// <summary>Reference to the Canvas of the current scene</summary>
    [SerializeField] private Canvas mainCanvas;

    /// <summary>Reference to Nova's wall collider</summary>
    [SerializeField] private GameObject wallCollider;
    /// <summary>Reference to Nova's damage collider</summary>
    [SerializeField] private GameObject damageCollider;
    /// <summary>Reference to Nova's button trigger, which allows</summary>
    [SerializeField] private GameObject buttonTrigger;

    /// <summary>Reference to the movable object Nova is currently standing on</summary>
    [HideInInspector] public Rigidbody2D MovableObject;

    /// <summary>Reference to the collision manager that disables collisions of first floor barriers to make a smoother jump possible.</summary>
    [SerializeField] private CollisionManager collisionManager;
    /// <summary>Reference to the ground tilemap. Used in the islands level to see if Nova is in an invalid tile</summary>
    [SerializeField] private Tilemap groundTilemap;

    /// <summary>Prefab of a charged bullet that can bounce off of walls</summary>
    [Header("Prefabs"), SerializeField] private GameObject chargedBulletPrefab;
    /// <summary>Prefab of a small bullet</summary>
    [SerializeField] private GameObject smallBulletPrefab;
    /// <summary>Prefab of the interaction prompt that gets displayed for interactions</summary>
    [SerializeField] private GameObject interacttionPromptPrefab;

    /// <summary>Reference to the charging audio source</summary>
    [Header("Audio Assets"), SerializeField] private AudioSource chargeAudioSource;
    /// <summary>Timer that keeps track of the time the attack button has been held</summary>
    private float chargeAttackTimer;
    /// <summary>The direction Nova will use to shoot a bullet</summary>
    private Vector2 attackDirection;
    /// <summary>Input value the game gets to make calculation for the attack direction</summary>
    private Vector2 initialAttackDirectionInput;

    /// <summary>List of interactable triggers Nova currently is in</summary>
    private List<InteractableTrigger> interactableTriggers;
    /// <summary>The interaction that will be / currently gets performed</summary>
    private InteractableTrigger performedInteraction;
    /// <summary>The current falling speed</summary>
    private float fallingSpeed;
    /// <summary>Tracks if Nova is currently falling</summary>
    public bool DoFall { get; set; }
    /// <summary>The sorting offset gets added on the normal sprite sorting index and is used when standing on top of movable chests</summary>
    private int sortingOffset;
    /// <summary>Area of the current movable box Nova stands on top of</summary>
    private BoxCollider2D firstFloorMovableBox;

    /// <summary>Tracks if the black fade currently is fading</summary>
    public bool Fading;
    /// <summary>Cache of the rigidbody velocity</summary>
    public Vector2 RbVelBuffer { get; set; }
    #endregion

    #region Methods
    /// <summary>Caches the movement input for Nova</summary>
    /// <param name="ctx">Callback context of the input action</param>
    private void ReadMovementInput(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (!ZeroGMovement)
            MoveInput = ctx.ReadValue<Vector2>() * MovementConstraint;
    }

    /// <summary>Does a ranged attack depending on how long the button has been pressed</summary>
    /// <param name="ctx">Callback context of the input action</param>
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

    /// <summary>Spawns a bullet and sets its velocity and floor</summary>
    /// <param name="bulletPrefab">The prefab that gets used to spawn a specific type of bullet</param>
    private void SpawnBullet(GameObject bulletPrefab)
    {
        var bulletGO = Instantiate(bulletPrefab);
        var bullet = bulletGO.GetComponent<ChargedBullet>();

        bulletGO.transform.position = ballSpawnPosition.position;
        bulletGO.layer = ballSpawnPosition.gameObject.layer;

        bullet.Rb.velocity = attackDirection.normalized * bullet.MovementSpeed;

        bulletGO.transform.parent = bulletContainer;

        if (ZeroGMovement) MoveInput = attackDirection.normalized;
    }

    /// <summary>Sets the attack direction for nova</summary>
    /// <param name="ctx">Callback context of the input action</param>
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

    /// <summary>Performs an interaction</summary>
    /// <param name="ctx">Callback context of the input action</param>
    private void DoInteract(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (GameManager.Instance.IsPlaying && !GameManager.Instance.IsFading)
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

    /// <summary>
    /// Shows the interaction prompt when the dialog finishes
    /// </summary>
    /// <param name="ctx">Callback context of the input action</param>
    private void ShowPromptWhenClosingDialog(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.UpdateInputScheme(ctx);

        if (GameManager.Instance.IsPlaying && ctx.action.WasReleasedThisFrame())
        {
            if (interactableTriggers.Contains(performedInteraction)) interactionPrompt.EnablePrompt(performedInteraction.InteractText, controls.Nova.Interact.bindings);
        }
    }

    /// <summary>Switches to the given floor. In case Nova jumps into an invalid tile, they respawn at their last valid position </summary>
    /// <param name="groundFloor">true makes Nova switch to the ground floor, false to the top floor</param>
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
                SetColliderLayers(3, 7, 8, 6);
            }
        }
        else
        {
            SetColliderLayers(13, 10, 11, 12);
        }
    }

    /// <summary>Sets the collision layers for the floor Nova is now in</summary>
    /// <param name="wall">Layer ID for walls</param>
    /// <param name="damage">Layer ID for damage objects</param>
    /// <param name="button">Layer ID for button interactions</param>
    /// <param name="bullets">Layer ID for bullets</param>
    private void SetColliderLayers(int wall, int damage, int button, int bullets)
    {
        wallCollider.layer = wall;
        damageCollider.layer = damage;
        buttonTrigger.layer = button;
        ballSpawnPosition.gameObject.layer = bullets;
    }

    /// <summary>Starts a jump</summary>
    public void BeginFall()
    {
        if (MovableObject != null) sortingOffset = 100;

        DoFall = true;

        fallingSpeed = 8f;
        fallTimer = fallTime;
        jumpDirection = MoveInput;

        if (collisionManager != null) collisionManager.DisableCollisions();
    }

    /// <summary>Stops a fall</summary>
    /// <param name="topLanding">true if Nova lands in an area specified as a first floor, false if they land on the ground floor</param>
    public void StopFall(bool topLanding = false)
    {
        if (topLanding)
        {
            switchFloor = false;
            var newFallTimer = fallTimer - fallTime / 4f;
            fallTimer = newFallTimer > 0 ? newFallTimer : fallTimer;
        }
        else
        {
            if (collisionManager != null) collisionManager.EnableCollisions();

            if (TransitionTrigger != null) // reenables the transition trigger that got used to jump off
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

    /// <summary>Sets the 0G state</summary>
    /// <param name="zeroG">True if Nova will use the 0G state, false if not</param>
    public void ToggleZeroG(bool zeroG)
    {
        ZeroGMovement = zeroG;
    }

    /// <summary>Tests if Nova is currently on valid ground</summary>
    /// <returns>True if Nova stands on ground, false if not</returns>
    public bool IsOnGround()
    {
        if (groundTilemap != null && !groundTilemap.HasTile(new Vector3Int((int)transform.position.x, (int)transform.position.y))) return false;
        else return true;
    }

    /// <summary>Resets Nova's position to the last valid recorded position</summary>
    public void Respawn()
    {
        transform.position = RespawnPosition;
    }

    /// <summary>Shows the interaction prompt of the current available interaction</summary>
    /// <param name="interactableTrigger">The interactable trigger that contains the information needed to display the prompt, including the text and position it should get displayed at</param>
    public void ShowInteractionPrompt(InteractableTrigger interactableTrigger)
    {
        if (!interactableTriggers.Contains(interactableTrigger))
        {
            interactableTriggers.Add(interactableTrigger);
            performedInteraction = interactableTriggers[0];

            if (interactionPrompt == null)
            {
                var go = Instantiate(interacttionPromptPrefab);
                interactionPrompt = go.GetComponent<InteractionPrompt>();
                interactionPrompt.transform.SetParent(mainCanvas.transform, false);
                interactionPrompt.transform.SetAsFirstSibling();
            }
            interactionPrompt.EnablePrompt(performedInteraction.InteractText, controls.Nova.Interact.bindings, performedInteraction.gameObject.transform);
        }
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// Initializes the controls and creates a bullet container if it doesn't exist, as well as setting some variables
    /// </summary>
    private void Awake()
    {
        controls ??= new();

        controls.Nova.Move.performed += ctx => ReadMovementInput(ctx);
        controls.Nova.RangedAttack.performed += ctx => DoRangedAttack(ctx);
        controls.Nova.RangedAttack.canceled += ctx => DoRangedAttack(ctx);

        controls.Nova.Aim.performed += ctx => SetAttackDirection(ctx);
        controls.Nova.Interact.performed += ctx => DoInteract(ctx);
        controls.Nova.Interact.canceled += ctx => DoInteract(ctx);
        controls.UI.ProceedDialog.canceled += ctx => ShowPromptWhenClosingDialog(ctx);

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

    /// <summary>
    /// Controls the charge attack timer, and the sorting order
    /// </summary>
    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            if (chargeAttackTimer >= 0)
            {
                chargeAttackTimer -= Time.deltaTime;
                animator.SetFloat("chargeTimer", chargeAttackTimer);
            }
            if (MovableObject != null && MovableObject.CompareTag("Movable Box"))
                animationSprites.sortingOrder = MovableObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
            else
                animationSprites.sortingOrder = -Mathf.RoundToInt(transform.position.y * 16) + sortingOffset;
        }
    }

    /// <summary>
    /// This Update Methods is all about movement and the animator
    /// </summary>
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
                animator.SetFloat("velocityX", 0f);
                animator.SetFloat("velocityY", 0f);
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

                var normalizedVelocity = (rb.velocity - MovableObject.velocity).normalized;
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

    /// <summary>
    /// Centers the camera on nova
    /// </summary>
    private void LateUpdate()
    {
        mainCamera.transform.position = new(transform.position.x, transform.position.y, mainCamera.transform.position.z);
    }

    /// <summary>
    /// Enables the controls
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>
    /// Disables the controls
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }

    /// <summary>
    /// When Nova enters a top floor from the movable box, they will be able to stand on top of it
    /// </summary>
    /// <param name="collision">The collider of the other object</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // movable box 1st floor
        if (collision.gameObject.CompareTag("1st Floor") && fallTimer > fallTime / 4f)
        {
            StopFall();
            firstFloorMovableBox = collision.gameObject.GetComponent<BoxCollider2D>();
            MovableObject = firstFloorMovableBox.GetComponentInParent<Rigidbody2D>();
        }
    }
    
    /// <summary>
    /// When leaving an interactable trigger, the performed interaction, in case it currently gets performed, will be ended.
    /// When leaving the area of the top of a movable box, nova will start to fall.
    /// </summary>
    /// <param name="collision"></param>
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
            interactionPrompt?.Hide();
        }
        else if (collision.gameObject.CompareTag("1st Floor") && firstFloorMovableBox != null)
        {
            firstFloorMovableBox.enabled = false;
            BeginFall();
            MovableObject = null;
        }
    }

    /// <summary>This helps make sure that I set up the character in new scenes correctly. It executes if values in the inspector change.</summary>
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
