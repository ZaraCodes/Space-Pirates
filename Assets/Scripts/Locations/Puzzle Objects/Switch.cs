using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : ToggleObject
{
    [Header("Switch Attributes"), SerializeField] private float activationTime = -1;

    private float timer;
    #region Methods

    #endregion

    #region Unity Stuff

    private void OnTriggerEnter2D(Collider2D collision)
    {
        State = !State;
        if (activationTime > 0f)
        {
            if (State) timer = activationTime;
            else timer = 0f;
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void Update()
    {
        if (activationTime > 0f && timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                State = !State;
            }
        }
    }

    private void Start()
    {
        timer = 0f;
    }


    #endregion
}
