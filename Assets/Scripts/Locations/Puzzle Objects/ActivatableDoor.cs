using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActivatableDoor : ActivatableObject
{
    #region Fields
    [Header("Door")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private BoxCollider2D trigger;
    public BoxCollider2D Trigger { get { return trigger; } set {  trigger = value; } }
    [Space]
    [SerializeField] private ActivatableDoor connectedDoor;
    public bool teleportEnabled;
    // public ActivatableDoor ConnectedDoor { get { return connectedDoor; } set { connectedDoor = value; } }
    #endregion

    #region Methods
    protected override void Toggle()
    {
        if (invertState)
        {
            spriteRenderer.sprite = State ? inactiveSprite : activeSprite;
            trigger.enabled = !State;
        }
        else
        {
            spriteRenderer.sprite = State ? activeSprite : inactiveSprite;
            trigger.enabled = State;
        }
        Debug.Log($"name: {name} State: {false}");
    }
    #endregion

    #region Unity Stuff
    private void Start()
    {
        Toggle();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //connectedDoor.teleportEnabled = false;
            var rb = collision.transform.parent.GetComponent<Rigidbody2D>();
            var angle = Vector2.Angle(rb.velocity, transform.right);

            if (rb.velocity.sqrMagnitude > 0 && angle < 45)
                collision.transform.parent.position = new Vector3(connectedDoor.transform.position.x, connectedDoor.transform.position.y + 0.5f, collision.transform.parent.position.z);
        }
    }

    private void OnValidate()
    {
        if (connectedDoor == null && name != "Door")
        {
            Debug.LogWarning($"Connected door field of GameObject {name} is null!");
        }
    }
    #endregion
}
