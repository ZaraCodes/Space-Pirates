using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A door transports Nova from one door to another, kind of like a teleporter. When it gets used, it can invoke an event.
/// </summary>
public class ActivatableDoor : ActivatableObject
{
    #region Fields
    /// <summary>Reference to the door's sprite renderer</summary>
    [Header("Door"), SerializeField] private SpriteRenderer spriteRenderer;
    /// <summary>Reference to the active sprite</summary>
    [SerializeField] private Sprite activeSprite;
    /// <summary>Reference to the inactive sprite</summary>
    [SerializeField] private Sprite inactiveSprite;
    /// <summary>Refernce to the box interaction trigger</summary>
    [SerializeField] private BoxCollider2D trigger;
    /// <summary>Refernce to the box interaction trigger</summary>
    public BoxCollider2D Trigger { get { return trigger; } set {  trigger = value; } }

    /// <summary>Unity Event that gets triggered when Nova uses a door</summary>
    public UnityEvent OnDoorUsed;
    
    /// <summary>Parent Object of the flying bullets. They get removed if Nova uses a door</summary>
    [Header("Must Have References"), SerializeField] private Transform BulletsParent;
    /// <summary>Reference to the connected door Nova will get teleported to</summary>
    [SerializeField] private ActivatableDoor connectedDoor;
    #endregion

    #region Methods
    /// <summary>Toggles the door on or off</summary>
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

    /// <summary>Coroutine that teleports Nova</summary>
    /// <param name="playerTransform">Transform of Nova</param>
    /// <param name="onFadeInDone">Unity Event that gets triggered once the black fade completely faded in</param>
    /// <returns></returns>
    private IEnumerator TeleportPlayer(Transform playerTransform, UnityEvent onFadeInDone)
    {
        var delay = .2f;

        playerTransform.position = new Vector3(connectedDoor.transform.position.x, connectedDoor.transform.position.y + 0.5f, playerTransform.position.z);
        ChargedBullet.PlayDestroySoundStatic = false;

        OnDoorUsed?.Invoke(); // triggers the door event, if there is one

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

    /// <summary>This helps make sure that I set up the door in new scenes correctly. It executes if values in the inspector change.</summary>
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
