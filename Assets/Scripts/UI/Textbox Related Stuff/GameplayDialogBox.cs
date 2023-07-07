using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GameplayDialogBox : MonoBehaviour
{
    #region Fields
    private int currentSequenceIndex;
    private int sequenceLength;
    [SerializeField] private float pauseTimeBetweenBoxes;
    private float pauseTimer = 0;

    [Header("References"), SerializeField] private TextMeshProUGUI content;
    [SerializeField] private Image characterPortrait;
    private TextboxSequence textboxSequence;
    private SpacePiratesControls controls;
    private string[] sectionStrings;

    private string visibleText;
    private string invisibleText;
    
    private int currentSectionIndex;
    private float timer;

    public UnityEvent OnTextboxClosed;
    #endregion

    #region Methods
    private void LoadDialog(string sequenceName)
    {
        textboxSequence = AllDialogs.Instance.GetSequence(sequenceName);

        if (textboxSequence == null)
        {
            Debug.LogWarning($"The dialog {sequenceName} does not exist!");
            return;
        }
        
        sequenceLength = textboxSequence.Contents.Length;
        sectionStrings = textboxSequence.Contents[currentSequenceIndex].SequenceText.GetLocalizedString().Split("\n||\n");
        currentSequenceIndex = 0;
        currentSectionIndex = -1;

        gameObject.SetActive(true);

        LoadNextBox();
    }

    private void LoadNextBox()
    {
        if (currentSectionIndex + 1 >= sectionStrings.Length)
        {
            currentSequenceIndex++;

            if (currentSequenceIndex < sequenceLength)
            {
                sectionStrings = textboxSequence.Contents[currentSequenceIndex].SequenceText.GetLocalizedString().Split("\n||\n");
                currentSectionIndex = 0;
            }
            else
            {
                gameObject.SetActive(false);

                OnTextboxClosed.Invoke();

                return;
            }
        }
        else
        {
            currentSectionIndex++;
        }
        invisibleText = sectionStrings[currentSectionIndex];
        visibleText = string.Empty;
        timer = 1 / SettingsS.Instance.TextboxSpeed * textboxSequence.Contents[currentSequenceIndex].TextSpeedMultiplier;
    }

    public void OnUINext(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
        {
            LoadNextBox();
        }
    }
    #endregion

    #region Unity Stuff
    private void Start()
    {
        LoadDialog("Test Sequence");
        timer = 0;
        pauseTimer = pauseTimeBetweenBoxes;
        //invisibleText = string.Empty;
        //visibleText = string.Empty;
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            while (timer <= 0 && invisibleText.Length > 0)
            {
                visibleText += invisibleText[0];
                invisibleText = invisibleText.Remove(0, 1);
                content.text = $"{visibleText}<color #FFF0>{invisibleText}";
                timer += 1 / SettingsS.Instance.TextboxSpeed * textboxSequence.Contents[currentSequenceIndex].TextSpeedMultiplier;
            }
            if (invisibleText.Length != 0 && timer > 0)
            {
                timer -= Time.deltaTime;
                if (pauseTimer != pauseTimeBetweenBoxes) pauseTimer = pauseTimeBetweenBoxes;
            }
            else if (pauseTimer > 0)
            {
                pauseTimer -= Time.deltaTime;
                if (pauseTimer <= 0)
                {
                    pauseTimer = 0;
                    LoadNextBox();
                }
            }
        }
    }

    private void Awake()
    {
        controls = new SpacePiratesControls();

        controls.UI.ProceedDialog.performed += ctx => OnUINext(ctx);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
