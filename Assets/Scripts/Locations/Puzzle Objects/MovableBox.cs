using UnityEngine;

/// <summary>
/// A movable box can be interacted with by Nova and moved either in a vertical or horizontal direction
/// </summary>
public class MovableBox : MonoBehaviour
{
    #region Fields
    /// <summary>This trigger handles horizontal movement of the box</summary>
    [SerializeField] private InteractableTrigger horizontalTrigger;
    /// <summary>This trigger handles vertical movement of the box</summary>
    [SerializeField] private InteractableTrigger verticalTrigger;
    /// <summary>Reference to the Movable Object this box may stand on. This is used to move the box along with a moving rafts for example</summary>
    [HideInInspector] public Rigidbody2D MovableObject;
    /// <summary>Reference to Nova's Rigidbody. This is used to make Nova move if they are standing on top of a box that is on top of a moving raft</summary>
    private Rigidbody2D novaRigidbody;
    /// <summary>Reference to the box's rigidbody</summary>
    [SerializeField] private Rigidbody2D boxRigidbody;
    /// <summary>Property that allows reading access to the rigidbox of this box</summary>
    public Rigidbody2D BoxRigidbody { get { return boxRigidbody; } }
    /// <summary>Reference to the sprite renderer of this box. It's needed because the box has to update it's sprite sorting index if it moves</summary>
    [SerializeField] private SpriteRenderer boxSprite;
    /// <summary>This is true if the box gets interacted with. It leads to the rigidbody being temorarily destroyed</summary>
    private bool triggered;
    #endregion

    #region Methods
    /// <summary>
    /// This gets executed while a box gets moved. To move the box, it becomes a child object of Nova for the duration of the interaction.
    /// </summary>
    /// <param name="horizontal">true if the box gets moved horizontally, false if vertically</param>
    public void Move(bool horizontal)
    {
        if (novaRigidbody != null || GameObject.Find("Nova").TryGetComponent(out novaRigidbody))
        {
            GameManager.Instance.Nova.Animator.SetBool("moveBox", true);
            SetSortingOrder();

            if (!triggered)
            {
                transform.parent = GameManager.Instance.Nova.transform;
                if (horizontal) GameManager.Instance.Nova.MovementConstraint = new(1, 0);
                else GameManager.Instance.Nova.MovementConstraint = new(0, 1);
            }
        }
    }

    /// <summary>
    /// Sets the distance of the box to Nova and destroys the rigidbody so it doesn't interfere with movement.
    /// </summary>
    public void SetDistanceToPlayer()
    {
        if (!triggered)
        {
            Destroy(boxRigidbody);
            triggered = true;
        }
    }

    /// <summary>
    /// Stops the movement and removes its relation to Nova's GameObject. Re-adds the rigidbody and removes the movement constraints from nova
    /// </summary>
    public void StopMoving()
    {
        triggered = false;
        transform.parent = null;

        if (boxRigidbody == null)
        {
            boxRigidbody = gameObject.AddComponent<Rigidbody2D>();
            boxRigidbody.mass = 100000000f;
            boxRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            boxRigidbody.gravityScale = 0f;
        }
        SetSortingOrder();
        GameManager.Instance.Nova.MovementConstraint = new(1, 1);
        GameManager.Instance.Nova.Animator.SetBool("moveBox", false);
    }

    /// <summary>
    /// Sets the sorting order to make the sprite sorting in game make sense
    /// </summary>
    private void SetSortingOrder()
    {
        boxSprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 16 - 8);
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// At the start of the scene the box sets its sorting order
    /// </summary>
    private void Start()
    {
        SetSortingOrder();
    }

    /// <summary>Adds the movement from the connected MovableObject if it exists and sets animation parameters for nova</summary>
    private void FixedUpdate()
    {
        if (MovableObject != null)
        {
            if (boxRigidbody != null)
            {
                boxRigidbody.velocity = MovableObject.velocity;
            }
        }

        if (transform.parent != null && transform.parent.CompareTag("Player"))
        {
            var distanceToPlayer = GameManager.Instance.Nova.transform.position - transform.position;
            GameManager.Instance.Nova.Animator.SetFloat("boxDiffX", distanceToPlayer.x);
            GameManager.Instance.Nova.Animator.SetFloat("boxDiffY", distanceToPlayer.y);
        }
    }
    #endregion

}
