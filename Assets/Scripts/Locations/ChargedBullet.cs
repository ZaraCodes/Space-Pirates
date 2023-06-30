using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;

public class ChargedBullet : MonoBehaviour
{
    /// <summary>Reference to the ball's rigidbody</summary>
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb { get { return rb; } }

    /// <summary>The speed at which the ball will move</summary>
    [SerializeField] private float movementSpeed;
    public float MovementSpeed { get { return movementSpeed; } }

    /// <summary>The maximum amount of bounces the ball will do before destroying itself.</summary>
    [SerializeField] private int maxBounces;
    
    /// <summary>The amount of bounces the ball performed.</summary>
    private int bounces;

    /// <summary>Velocity of the ball</summary>
    private Vector2 velocity;

    [Header("Audio Assets")]
    [SerializeField] private AudioMixerGroup audioMixerGroup;

    [SerializeField] private AudioClip[] bounceSounds;
    [SerializeField] private AudioClip destroyedSound;


    /// <summary>
    /// Creates an audio source at the position of the collision and plays a sound
    /// </summary>
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
        selfDestruct.lifetime = lifetime;
    }

    #region Unity Stuff

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

    private void Start()
    {
        bounces = 0;
    }

    private void FixedUpdate()
    {
        velocity = rb.velocity;
    }

    private void OnDestroy()
    {
        SpawnAudioSource(.2f, destroyedSound);

        //Todo: Play ball destroyed animation.
    }


    #endregion
}
   