using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TextboxTrigger : MonoBehaviour
{
    #region Fields
    [SerializeField] private string DialogID;
    [SerializeField] private bool gameplayDialog;
    #endregion
    #region Unity Stuff

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameplayDialog)
            {
                //Todo: Test for progression / story flag

                GameManager.Instance.GameplayDialog.LoadDialog(DialogID);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("meow");
    }
    #endregion
}
