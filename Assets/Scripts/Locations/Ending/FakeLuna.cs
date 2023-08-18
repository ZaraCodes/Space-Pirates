using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeLuna : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// Reference to the cat's animator
    /// </summary>
    [SerializeField] private Animator animator;

    /// <summary>
    /// Reference to the rigidbody that moves the cat
    /// </summary>
    [SerializeField] private Rigidbody2D rb;

    /// <summary>
    /// The speed fake luna moves with
    /// </summary>
    [SerializeField] private float movementSpeed;

    /// <summary>
    /// Reference to the captain meow confrontation dialog
    /// </summary>
    [SerializeField] private TextboxTrigger captainMeowDialog;

    /// <summary>
    /// Reference to the audio source that plays the music for captain meow
    /// </summary>
    [SerializeField] private AudioSource captainMeowMusic;

    /// <summary>
    /// Reference to the audio source that plays the music for the moon
    /// </summary>
    [SerializeField] private AudioSource moonMusic;

    #endregion

    #region Methods
    /// <summary>
    /// Starts moving the cat in the given direction
    /// </summary>
    /// <param name="direction">The movement direction for the cat</param>
    public void StartMoving(Vector2 direction)
    {
        rb.velocity = direction.normalized * movementSpeed;
        animator.SetBool("Walk", true);
    }

    /// <summary>Starts the teleport animation for the cat and the final dialog</summary>
    public void Teleport()
    {
        animator.SetTrigger("Teleport");
        StartCoroutine(DelayCaptainMeowDialog());
        StartCoroutine(CrossFadeMusic());
    }

    /// <summary>Coroutine that delays the final dialog until after the animation has finished</summary>
    /// <returns></returns>
    public IEnumerator DelayCaptainMeowDialog()
    {
        GameManager.Instance.IsSceneIntroPlaying = true;

        yield return new WaitForSeconds(2);
        GameManager.Instance.IsSceneIntroPlaying = false;
        captainMeowDialog.LoadDialog();
    }

    /// <summary>Coroutine that cross fades the music on the moon</summary>
    /// <returns></returns>
    private IEnumerator CrossFadeMusic()
    {
        captainMeowMusic.Play();
        var limit = .4f;
        var timer = 4f;
        var maxTime = timer;

        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
            captainMeowMusic.volume = (1 - (timer / maxTime)) * limit;
            moonMusic.volume = (timer / maxTime) * limit;
        }
        yield return null;
        moonMusic.Stop();
    }
    #endregion

    #region Unity Stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        rb.velocity = Vector2.zero;
        if (collision.name == "Waypoint 1")
        {
            animator.SetTrigger("Waypoint 1");
            animator.SetBool("Walk", false);
        }
        else if (collision.name == "Waypoint 0")
        {
            animator.SetTrigger("Waypoint 0");
            animator.SetBool("Walk", false);
        }
    }
    #endregion
}
