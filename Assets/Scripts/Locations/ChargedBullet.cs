using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// A bullet that gets fired by Nova. It has the ability to bounce off of walls.
/// </summary>
public class ChargedBullet : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the ball's rigidbody</summary>
    [SerializeField] private Rigidbody2D rb;

    /// <summary>Property that allows reading access to the rigidbody of the bullet</summary>
    public Rigidbody2D Rb { get { return rb; } }

    /// <summary>Reference to the sprite renderer</summary>
    [SerializeField] private SpriteRenderer spriteRenderer;

    /// <summary>The speed at which the ball will move</summary>
    [SerializeField] private float movementSpeed;
    /// <summary>Property that allows reading access to the rigidbody's movement speed</summary>
    public float MovementSpeed { get { return movementSpeed; } }

    /// <summary>The maximum amount of bounces the ball will do before destroying itself.</summary>
    [SerializeField] private int maxBounces;
    
    /// <summary>The amount of bounces the ball performed.</summary>
    private int bounces;

    /// <summary>Velocity of the ball</summary>
    private Vector2 velocity;

    /// <summary>Reference to the audio mixer group for sound effects</summary>
    [Header("Audio Assets"), SerializeField] private AudioMixerGroup audioMixerGroup;
    /// <summary>Array of sound clips for bouncing off walls</summary>
    [SerializeField] private AudioClip[] bounceSounds;
    /// <summary>Audio Clip for destroying a bullet</summary>
    [SerializeField] private AudioClip destroyedSound;

    /// <summary>If false, all bullets will not play a sound when it gets destroyed</summary>
    public static bool PlayDestroySoundStatic;
    /// <summary>If false, this bullet will not play a sound when it gets destroyed</summary>
    public bool PlayDestroySound;
    #endregion

    #region Methods
    /// <summary>Creates an audio source at the position of the collision and plays a sound</summary>
    /// <param name="lifetime">The time for how long this audio clip will exist in the scene before it gets destroyed</param>
    /// <param name="soundClip">The audio clip that will get played</param>
    private void SpawnAudioSource(float lifetime, AudioClip soundClip)
    {
        var audioSourceGO = new GameObject();
        audioSourceGO.transform.position = transform.position;

        var audioSource = audioSourceGO.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.spatialBlend = .8f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 100;
        audioSource.Play();

        var selfDestruct = audioSourceGO.AddComponent<SelfDestructionScript>();
        selfDestruct.Lifetime = lifetime;
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// When the bullet enters a collision (like a wall), it will add 1 to its performed bounces. If that reaches the max allowed bounces, the bullet gets destroyed
    /// </summary>
    /// <param name="collision">The collision of the other object</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (++bounces >= maxBounces)
        {
            Destroy(gameObject);
        }
        else
        {
            SpawnAudioSource(.2f, bounceSounds[Random.Range(0, bounceSounds.Length - 1)]);

            var contactPoint = collision.GetContact(0);
            var newVelocity = Vector2.Reflect(velocity, contactPoint.normal);

            rb.velocity = newVelocity;
        }
    }

    /// <summary>
    /// While it exists, the bullet continuously updates its sorting order
    /// </summary>
    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 16) + 1;
        }
    }

    /// <summary>
    /// Sets up the parameters for the bullet
    /// </summary>
    private void Start()
    {
        bounces = 0;
        PlayDestroySoundStatic = true;
        PlayDestroySound = true;
        if (transform.parent == null)
        {
            Debug.LogWarning("Please make sure that the object that spawns this bullet gives the bullets it spawns a parent GameObject with the name \"Bullets\"");
            transform.parent = GameObject.Find("Bullets").transform;
        }
        var selfDestruct = gameObject.AddComponent<SelfDestructionScript>();
        selfDestruct.Lifetime = 20f;
        selfDestruct.OnTimerEnded += () => PlayDestroySound = false;
    }

    /// <summary>
    /// Caches the current velocity
    /// </summary>
    private void FixedUpdate()
    {
        velocity = rb.velocity;
    }

    /// <summary>
    /// Plays an audio clip when the bullet gets destroyed if the conditions are met
    /// </summary>
    private void OnDestroy()
    {
        if (PlayDestroySoundStatic && PlayDestroySound)
        {
            SpawnAudioSource(.2f, destroyedSound);
        }
        //Todo: Play ball destroyed animation.
    }
    #endregion
}
   