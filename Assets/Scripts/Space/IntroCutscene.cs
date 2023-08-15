using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntroCutscene : MonoBehaviour
{
    #region Fields
    /// <summary>The velocity the ship starts with in the intro cutscene</summary>
    [SerializeField] private Vector2 startingVelocity;

    /// <summary>The cutscene's space ship's rigidbody</summary>
    [SerializeField] private Rigidbody2D shipRigidbody;

    [SerializeField] private TextboxTrigger preMeowDialog;
    #endregion

    #region Methods
    /// <summary>Starts the ship movement before the dialog</summary>
    private void StartShipMovement()
    {
        shipRigidbody.velocity = startingVelocity;
    }

    
    #endregion

    #region Unity Stuff
    private void Start()
    {
        StartShipMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("meow");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("myaa"); 
        UnityEvent onFadeInFinished = new();
        onFadeInFinished.AddListener(() =>
        {
            preMeowDialog.LoadDialog();
        });
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(onFadeInFinished));
    }
    #endregion
}
