using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ActivatableDoor : ActivatableObject
{
    #region Fields
    [Header("Door")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private BoxCollider2D trigger;
    public BoxCollider2D Trigger { get { return trigger; } set {  trigger = value; } }

    public UnityEvent OnDoorUsed;

    [Space]
    [Header("Must Have References"), SerializeField] private Transform BulletsParent;
    [SerializeField] private ActivatableDoor connectedDoor;
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
        // Debug.Log($"name: {name} State: {false}");
    }

    /// <summary>Teleports the player and removes all bullets</summary>
    /// <param name="playerTransform">The transform of the player</param>
    private void TeleportPlayer(Transform playerTransform)
    {
        if (connectedDoor == null) return;

        GameManager.Instance.Nova.Fading = true;
        UnityEvent onFadeInDone = new();
        onFadeInDone.AddListener(() =>
        {
            StartCoroutine(TeleportPlayer(playerTransform, onFadeInDone));
        });
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(onFadeInDone));

    }

    private IEnumerator TeleportPlayer(Transform playerTransform, UnityEvent onFadeInDone)
    {
        var delay = .2f;
        //yield return new WaitForSeconds(delay);

        playerTransform.position = new Vector3(connectedDoor.transform.position.x, connectedDoor.transform.position.y + 0.5f, playerTransform.position.z);
        ChargedBullet.playDestroySoundStatic = false;

        OnDoorUsed?.Invoke();

        foreach (Transform t in BulletsParent)
        {
            if (t.TryGetComponent<ChargedBullet>(out var bullet))
            {
                Destroy(bullet.gameObject);
            }
        }
        yield return new WaitForSeconds(delay);
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeOut(null));

        onFadeInDone.RemoveAllListeners();
    }
    #endregion

    #region Unity Stuff
    private void Start()
    {
        Toggle();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Button Trigger") && collision.name.Contains("Nova"))
        {
            //connectedDoor.teleportEnabled = false;
            var rb = collision.transform.parent.GetComponent<Rigidbody2D>();
            var angle = Vector2.Angle(rb.velocity, transform.right);

            if (rb.velocity.sqrMagnitude > 0 && angle < 45 && !GameManager.Instance.Nova.Fading) TeleportPlayer(collision.transform.parent);
        }
    }

    private void OnValidate()
    {
        if (gameObject.scene.name != null)
        {

            if (connectedDoor == null)
            {
                Debug.LogWarning($"GameObject {name}: Connected door is null!");
            }
            if (BulletsParent == null)
            {
                Debug.LogWarning($"GameObject {name}: Bullets Reference is null!");
            }
        }
    }
    #endregion
}
