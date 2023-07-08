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
    private float textCompletionMultiplier;
    #endregion

    #region Methods
    public void LoadDialog(string sequenceName)
    {
        textboxSequence = AllDialogs.Instance.GetSequence(sequenceName);

        if (textboxSequence == null)
        {
            Debug.LogWarning($"The dialog \"{sequenceName}\" does not exist!");
            return;
        }
        
        currentSequenceIndex = 0;
        currentSectionIndex = -1;
        textCompletionMultiplier = 1;

        sequenceLength = textboxSequence.Contents.Length;
        sectionStrings = textboxSequence.Contents[currentSequenceIndex].SequenceText.GetLocalizedString().Split("\n||\n");

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
                textboxSequence.Contents[currentSequenceIndex].OnContentFinished.Invoke();

                sectionStrings = textboxSequence.Contents[currentSequenceIndex].SequenceText.GetLocalizedString().Split("\n||\n");
                currentSectionIndex = 0;
            }
            else
            {
                gameObject.SetActive(false);

                // OnTextboxClosed.Invoke();

                return;
            }
        }
        else
        {
            currentSectionIndex++;
        }
        invisibleText = sectionStrings[currentSectionIndex];
        characterPortrait.sprite = textboxSequence.Contents[currentSequenceIndex].Speaker.Portrait;
        content.text = string.Empty;
        content.color = textboxSequence.Contents[currentSequenceIndex].Speaker.TextColor;
        visibleText = string.Empty;
        textCompletionMultiplier = 1;
        timer = 1 / SettingsS.Instance.TextboxSpeed * textboxSequence.Contents[currentSequenceIndex].TextSpeedMultiplier;
    }

    public void OnUINext(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
        {
            if (invisibleText.Length > 0) textCompletionMultiplier = 0;
            else LoadNextBox();
        }
    }
    #endregion

    #region Unity Stuff
    private void Start()
    {
        // LoadDialog("Test Sequence");
        timer = 0;
        pauseTimer = pauseTimeBetweenBoxes;
        //invisibleText = string.Empty;
        //visibleText = string.Empty;
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying && invisibleText is not null)
        {
            while (timer <= 0 && invisibleText.Length > 0)
            {
                visibleText += invisibleText[0];
                invisibleText = invisibleText.Remove(0, 1);
                content.text = $"{visibleText}<color #FFF0>{invisibleText}";
                timer += 1f / SettingsS.Instance.TextboxSpeed * textboxSequence.Contents[currentSequenceIndex].TextSpeedMultiplier * textCompletionMultiplier;
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