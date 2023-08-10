using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI buttonPrompt;

    private TextboxSequence textboxSequence;
    private SpacePiratesControls controls;
    private string[] sectionStrings;

    private string visibleText;
    private string invisibleText;
    
    private int currentSectionIndex;
    private float timer;
    private float textCompletionMultiplier;

    public UnityEvent OnTextboxClosed;
    #endregion

    #region Methods
    public void LoadDialog(TextboxSequence textboxSequence, UnityEvent onTextboxClosed)
    {
        this.textboxSequence = textboxSequence;
        OnTextboxClosed = onTextboxClosed;

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

                OnTextboxClosed?.Invoke();

                return;
            }
        }
        else
        {
            currentSectionIndex++;
        }
        HideButtonPrompt();

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

    private void ShowButtonPrompt()
    {
        buttonPrompt.gameObject.SetActive(true);
        buttonPrompt.text = $"<sprite name=\"{InputIconStringSetter.GetIconStringFromBinding(controls.UI.ProceedDialog.bindings)}\">";
    }

    private void HideButtonPrompt()
    {
        buttonPrompt.gameObject.SetActive(false);
    }

    public void UpdateIcon(EInputScheme scheme)
    {
        string buttonName = InputIconStringSetter.GetIconStringFromBinding(controls.UI.ProceedDialog.bindings);

        //Todo: test for availability of button sprite
        //bool noMatchFound = true;
        //foreach (var buttonIcon in buttonIcons.spriteInfoList)
        //{
        //    if (buttonIcon.name == buttonName)
        //    {
        //        noMatchFound = false;
        //    }
        //}
        //if (noMatchFound)
        //{
        //    interactText.text = $"[] {promptText.GetLocalizedString()}";
        //}
        //else
        buttonPrompt.text = $"<sprite name=\"{buttonName}\">";
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
                if (pauseTimer != pauseTimeBetweenBoxes)
                {
                    pauseTimer = pauseTimeBetweenBoxes;
                }
            }
            else if (pauseTimer > 0)
            {
                if (!buttonPrompt.gameObject.activeInHierarchy) ShowButtonPrompt();

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
        controls = new();

        controls.UI.ProceedDialog.performed += ctx => OnUINext(ctx);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnInputSchemeChanged += newScheme => UpdateIcon(newScheme);
        controls.Enable();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnInputSchemeChanged -= newScheme => UpdateIcon(newScheme);
        controls.Disable();
    }
    #endregion
}
