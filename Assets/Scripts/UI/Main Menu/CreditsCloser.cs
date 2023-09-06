using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreditsCloser : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the main menu</summary>
    [SerializeField] private GameObject mainMenu;
    /// <summary>Reference to the credits</summary>
    [SerializeField] private GameObject credits;
    /// <summary>Reference to the script that ends the credits</summary>
    [SerializeField] private CreditsEnding creditsEnding;

    /// <summary>Should the main menu scene be loaded</summary>
    [SerializeField] private bool loadMainMenu;

    private SpacePiratesControls controls;
    #endregion

    #region Methods
    /// <summary>
    /// Closes the credits 
    /// </summary>
    /// <param name="ctx"></param>
    private void CloseCredits(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
        {
            if (loadMainMenu)
            {
                creditsEnding.LoadMainMenu();
            }
            else
            {
                mainMenu.SetActive(true);
                credits.SetActive(false);
            }
        }
    }
    #endregion

    #region Unity Stuff
    private void Awake()
    {
        controls = new();
        controls.UI.UIBack.performed += CloseCredits;
    }

    /// <summary>
    /// Enables the controls
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>
    /// Disables the controls
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
