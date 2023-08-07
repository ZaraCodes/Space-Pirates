using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : ToggleObject
{
    [Header("Switch Attributes"), SerializeField] private float activationTime = -1;
    [SerializeField] private bool disableOnActivate;

    private float timer;
    #region Methods
    public void Toggle()
    {
        State = !State;
        if (activationTime > 0f)
        {
            if (State) timer = activationTime;
            else timer = 0f;
        }
    }

    public void SetState(bool state)
    {
        State = state;
    }
    #endregion

    #region Unity Stuff

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(State && disableOnActivate))
        {
            Toggle();

            if (collision.gameObject.CompareTag("Bullet"))
            {
                Destroy(collision.gameObject);
            }
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
