using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBox : MonoBehaviour
{
    [SerializeField] private InteractableTrigger horizontalTrigger;
    [SerializeField] private InteractableTrigger verticalTrigger;
    [HideInInspector] public Rigidbody2D MovableObject;

    private Rigidbody2D novaRigidbody;
    [SerializeField] private Rigidbody2D boxRigidbody;
    public Rigidbody2D BoxRigidbody { get { return boxRigidbody; } }
    [SerializeField] private SpriteRenderer boxSprite;

    private Vector2 moveVector;
    private Vector3 distanceToPlayer;

    bool triggered;

    #region Methods
    public void Move(bool horizontal)
    {
        if (novaRigidbody != null || GameObject.Find("Nova").TryGetComponent(out novaRigidbody))
        {
            GameManager.Instance.Nova.Animator.SetBool("moveBox", true);

            if (horizontal)
            {
                moveVector = new Vector2(novaRigidbody.velocity.x, 0);
            }
            else
            {
                moveVector = new Vector2(0, novaRigidbody.velocity.y);
            }
            
            if (!triggered)
            {
                transform.parent = GameManager.Instance.Nova.transform;
                if (horizontal) GameManager.Instance.Nova.MovementConstraint = new(1, 0);
                else GameManager.Instance.Nova.MovementConstraint = new(0, 1);
            }
        }
    }

    public void SetDistanceToPlayer()
    {
        if (!triggered)
        {
            Destroy(boxRigidbody);
            triggered = true;
            distanceToPlayer = transform.localPosition;
        }
    }

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
        GameManager.Instance.Nova.MovementConstraint = new(1, 1);
        GameManager.Instance.Nova.Animator.SetBool("moveBox", false);
    }
    #endregion

    #region Unity Stuff
    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            boxSprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 16 - 8);
        }
    }

    private void FixedUpdate()
    {
        // boxRigidbody.velocity = moveVector;
        if (MovableObject != null)
        {
            if (boxRigidbody != null)
                boxRigidbody.velocity = MovableObject.velocity;
        }

        if (transform.parent != null && transform.parent.name == "Nova")
        {
            var distanceToPlayer = GameManager.Instance.Nova.transform.position - transform.position;
            GameManager.Instance.Nova.Animator.SetFloat("boxDiffX", distanceToPlayer.x);
            GameManager.Instance.Nova.Animator.SetFloat("boxDiffY", distanceToPlayer.y);
        }
    }
    #endregion

}
