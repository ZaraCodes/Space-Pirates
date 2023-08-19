using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// This type of dialog displays during gameplay and does not interrupt it
/// </summary>
public class GameplayDialogBox : MonoBehaviour
{
    #region Fields
    /// <summary>Index of the current text sequence</summary>
    private int currentSequenceIndex;
    /// <summary>Length of the text sequence</summary>
    private int sequenceLength;
    /// <summary>The time the box pauses before automatically continueing</summary>
    [SerializeField] private float pauseTimeBetweenBoxes;
    /// <summary>Timer that keeps track of the pause time</summary>
    private float pauseTimer = 0;

    /// <summary>The text content of the dialog message</summary>
    [Header("References"), SerializeField] private TextMeshProUGUI content;
    /// <summary>reference to the character portrait</summary>
    [SerializeField] private Image characterPortrait;
    /// <summary>Reference to the button prompt</summary>
    [SerializeField] private TextMeshProUGUI buttonPrompt;

    /// <summary>The textbox sequence that gets displayed</summary>
    private TextboxSequence textboxSequence;
    /// <summary>/// Reference to the controls</summary>
    private SpacePiratesControls controls;
    /// <summary>Array of the strings of the currect section. when a section has finished, the next section gets displayed, which most often is a new speaker</summary>
    private string[] sectionStrings;

    /// <summary>The text that is visible in the message</summary>
    private string visibleText;
    /// <summary>The text that is invisible in the message</summary>
    private string invisibleText;
    
    /// <summary>Index within the current section</summary>
    private int currentSectionIndex;
    /// <summary>Timer that keeps makes the text appear bit by bit</summary>
    private float timer;
    /// <summary>/// Text speed multiplier that's used to instantly fill the box with the rest of the invisible text</summary>
    private float textCompletionMultiplier;

    /// <summary>Event that gets triggered after the textbox sequence has finished</summary>
    public UnityEvent OnTextboxClosed;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes the dialog
    /// </summary>
    /// <param name="dialogSequence">the dialog sequence that will get displayed</param>
    /// <param name="onDialogFinished">Unity Event that gets triggered when a dialog finishes</param>
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

    /// <summary>
    /// Loads the next message and initializes it.
    /// If the end of the dialog sequence has been reached, the overlay closes and triggeres the dialog finished event
    /// </summary>
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

    /// <summary>Takes the input from the game and either fills the message completely if it isn't completed yet or loads the next message</summary>
    /// <param name="ctx">Callback Context from the input action</param>
    public void OnUINext(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
        {
            if (invisibleText.Length > 0) textCompletionMultiplier = 0;
            else LoadNextBox();
        }
    }

    /// <summary>Shows the button prompt</summary>
    private void ShowButtonPrompt()
    {
        buttonPrompt.gameObject.SetActive(true);
        buttonPrompt.text = $"<sprite name=\"{InputIconStringSetter.GetIconStringFromBinding(controls.UI.ProceedDialog.bindings)}\">";
    }

    /// <summary>
    /// Hides the button prompt
    /// </summary>
    private void HideButtonPrompt()
    {
        buttonPrompt.gameObject.SetActive(false);
    }

    /// <summary>Updates the input prompt</summary>
    /// <param name="scheme">The new input scheme</param>
    public void UpdateIcon(EInputScheme scheme)
    {
        string buttonName = InputIconStringSetter.GetIconStringFromBinding(controls.UI.ProceedDialog.bindings);

        buttonPrompt.text = $"<sprite name=\"{buttonName}\">";
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// Initializes the box
    /// </summary>
    private void Start()
    {
        timer = 0;
        pauseTimer = pauseTimeBetweenBoxes;
    }

    /// <summary>
    /// Handles filling the textbox with visible text and automatically proceeding
    /// </summary>
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

    /// <summary>
    /// Initializes the controls
    /// </summary>
    private void Awake()
    {
        controls = new();

        controls.UI.ProceedDialog.performed += ctx => OnUINext(ctx);
    }

    /// <summary>
    /// Enables the controls and adds itself to the input scheme changed event
    /// </summary>
    private void OnEnable()
    {
        GameManager.Instance.OnInputSchemeChanged += newScheme => UpdateIcon(newScheme);
        controls.Enable();
    }

    /// <summary>
    /// Disables the controls and removes itself from the input scheme changed event
    /// </summary>
    private void OnDisable()
    {
        GameManager.Instance.OnInputSchemeChanged -= newScheme => UpdateIcon(newScheme);
        controls.Disable();
    }
    #endregion
}
