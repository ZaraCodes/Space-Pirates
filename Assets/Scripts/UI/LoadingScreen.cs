using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    #region Fields
    [SerializeField] private Animator iconAnimator;
    #endregion

    #region Unity Stuff
    private void OnEnable()
    {
        switch (Random.Range(0, 8))
        {
            case 0:
                iconAnimator.SetBool("LookAround", true); break;
            case 1:
                iconAnimator.SetBool("Clean1", true); break;
            case 2:
                iconAnimator.SetBool("Clean2", true); break;
            case 3:
                iconAnimator.SetBool("Walk1", true); break;
            case 4:
                iconAnimator.SetBool("Walk2", true); break;
            case 5:
                iconAnimator.SetBool("Sleep", true); break;
            case 6:
                iconAnimator.SetBool("Poke", true); break;
            case 7:
                iconAnimator.SetBool("Jump", true); break;
            default:
                iconAnimator.SetBool("Stretch", true); break;
        }
    }

    private void OnDisable()
    {
        iconAnimator.SetBool("LookAround", false);
        iconAnimator.SetBool("Clean1", false);
        iconAnimator.SetBool("Clean2", false);
        iconAnimator.SetBool("Walk1", false);
        iconAnimator.SetBool("Walk2", false);
        iconAnimator.SetBool("Sleep", false);
        iconAnimator.SetBool("Poke", false);
        iconAnimator.SetBool("Jump", false);
        iconAnimator.SetBool("Stretch", false);
    }
    #endregion
}
