using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class ShootTutorial : MonoBehaviour
{
    /// <summary>Reference to the aim prompt</summary>
    [SerializeField] private InteractionPrompt aimPrompt;
    /// <summary>Reference to the shoot prompt</summary>
    [SerializeField] private InteractionPrompt shootPrompt;
    /// <summary>the localized text for the aim prompt</summary>
    [SerializeField] private LocalizedString aimText;
    /// <summary>the localized text for the shoot prompt</summary>
    [SerializeField] private LocalizedString shootText;

    /// <summary>Cached reference to the controls for nova</summary>
    private SpacePiratesControls controls;

    /// <summary>Shows the shoot tutorial prompts</summary>
    public void ShowTutorial()
    {
        if (!ProgressionManager.Instance.Flags.Contains(EProgressionFlag.RepairKit1))
        {
            gameObject.SetActive(true);
            aimPrompt.EnablePrompt(aimText, controls.Nova.Aim.bindings);
            shootPrompt.EnablePrompt(shootText, controls.Nova.RangedAttack.bindings);
        }
    }

    /// <summary>Hides the shoot tutorial prompts</summary>
    public void HideTutorial()
    {
        gameObject.SetActive(false);
    }

    #region Unity Stuff
    private void Start()
    {
        controls = GameManager.Instance.Nova.Controls;
    }
    #endregion
}
