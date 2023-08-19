using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// The dialog overlay is the main method used to display conversations between characters
/// </summary>
public class DialogOverlay : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the controls</summary>
    private SpacePiratesControls controls;

    /// <summary>initial position of the scroll object</summary>
    private Vector3 originalScrollPosition;

    /// <summary>index of the text within the current section</summary>
    private int currentSectionIndex;
    /// <summary>index of the current section within the entire dialog sequence</summary>
    private int currentSequenceIndex;
    /// <summary>Length of the dialog sequence</summary>
    private int sequenceLength;
    /// <summary>The dialog sequence that gets displayed in the dialog overlay</summary>
    private TextboxSequence dialogSequence;
    /// <summary>Array of the text from the current section within the sequence</summary>
    private string[] sectionStrings;

    /// <summary>Reference to the current dialog message</summary>
    private DialogMessage currentDialogMessage;

    /// <summary>Timer that takes care of making the text appear bit by bit</summary>
    private float timer;

    /// <summary>previous alignment of the current message</summary>
    private bool previousAlignment;
    /// <summary>current alignment of the current message</summary>
    private bool currentAlignment;
    /// <summary>previous name of the current speaker</summary>
    private string previousName;
    /// <summary>current name of the current speaker</summary>
    private string currentName;
    /// <summary>previous positioning offset</summary>
    private float previousOffset;

    /// <summary>the total distance scrolled (from the scrolling object)</summary>
    private float scrolledDistance;

    /// <summary>keeps track of if the previous message displayed the portrait</summary>
    private bool avatarDisplayedInLastBox;

    /// <summary>Reference to the scroll field object that is the parent object of the messages</summary>
    [Header("References"), SerializeField] private GameObject scrollField;
    /// <summary>audio source that plays a click sound when the dialog proceeds</summary>
    [SerializeField] private AudioSource clickSource;
    /// <summary>Reference to the button prompt</summary>
    [SerializeField] private TextMeshProUGUI buttonPrompt;

    /// <summary>Prefab of a message that's attached to the left side of the screen</summary>
    [Header("Prefabs"), SerializeField] private GameObject leftMessagePrefab;
    /// <summary>Prefab of a message that's attached to the right side of the screen</summary>
    [SerializeField] private GameObject rightMessagePrefab;

    /// <summary>Unity Event that gets triggered when a dialog finishes</summary>
    public UnityEvent OnDialogFinished;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes the dialog
    /// </summary>
    /// <param name="dialogSequence">the dialog sequence that will get displayed</param>
    /// <param name="onDialogFinished">Unity Event that gets triggered when a dialog finishes</param>
    public void LoadDialog(TextboxSequence dialogSequence, UnityEvent onDialogFinished)
    {
        this.dialogSequence = dialogSequence;
        OnDialogFinished = onDialogFinished;
        GameManager.Instance.IsDialogOverlayOpen = true;

        avatarDisplayedInLastBox = false;
        scrolledDistance = originalScrollPosition.y;

        currentSequenceIndex = 0;
        currentSectionIndex = -1;

        sequenceLength = dialogSequence.Contents.Length;
        sectionStrings = dialogSequence.Contents[currentSequenceIndex].SequenceText.GetLocalizedString().Split("\n||\n");

        previousAlignment = !dialogSequence.Contents[currentSequenceIndex].positionedLeft;
        previousName = dialogSequence.Contents[currentSequenceIndex].Speaker.SpeakerName.GetLocalizedString() + "c";

        gameObject.SetActive(true);

        LoadNextBox();
    }

    /// <summary>
    /// Loads the next message and initializes it. depending on the previous message, the portrait and the name will be hidden.
    /// If the end of the dialog sequence has been reached, the overlay closes and triggeres the dialog finished event
    /// </summary>
    private void LoadNextBox()
    {
        if (currentSectionIndex + 1 >= sectionStrings.Length)
        {
            currentSequenceIndex++;

            if (currentSequenceIndex < sequenceLength)
            {
                dialogSequence.Contents[currentSequenceIndex].OnContentFinished.Invoke();
               
                sectionStrings = dialogSequence.Contents[currentSequenceIndex].SequenceText.GetLocalizedString().Split("\n||\n");
                currentSectionIndex = 0;
            }
            else
            {
                RemoveAllMessages();
                scrollField.transform.position = new(scrollField.transform.position.x, originalScrollPosition.y);
                gameObject.SetActive(false);
                GameManager.Instance.IsDialogOverlayOpen = false;
                OnDialogFinished?.Invoke();
                return;
            }
        }
        else
        {
            currentSectionIndex++;
        }

        currentDialogMessage = InstanciateDialogMessage();

        var scrollingOffset = currentDialogMessage.InitMessage(
            sectionStrings[currentSectionIndex],
            dialogSequence.Contents[currentSequenceIndex].Speaker.SpeakerName.GetLocalizedString(),
            dialogSequence.Contents[currentSequenceIndex].Speaker.TextColor,
            dialogSequence.Contents[currentSequenceIndex].Speaker.NameColor,
            dialogSequence.Contents[currentSequenceIndex].TextSpeedMultiplier
            );

        if (currentAlignment == previousAlignment && currentName == previousName)
        {
            if (avatarDisplayedInLastBox)
            {
                previousOffset = scrollingOffset * SettingsS.Instance.UIScale;

                if (scrollingOffset * SettingsS.Instance.UIScale < 60 * SettingsS.Instance.UIScale)
                {
                    //scrollingOffset = 60 * SettingsS.Instance.UIScale;
                    scrollingOffset = (scrollingOffset + 25) * SettingsS.Instance.UIScale;
                    previousOffset = scrollingOffset;
                }
                else scrollingOffset *= SettingsS.Instance.UIScale;
                avatarDisplayedInLastBox = false;
            }
            else if (previousOffset > (scrollingOffset + 25) * SettingsS.Instance.UIScale)
            {
                scrollingOffset = (scrollingOffset + 25) * SettingsS.Instance.UIScale;
                previousOffset = scrollingOffset;
            }
            else
            {
                scrollingOffset = (scrollingOffset + 10) * SettingsS.Instance.UIScale;
                previousOffset = scrollingOffset;
            }
            currentDialogMessage.PortraitFrame.SetActive(false);
        }
        else
        {
            scrollingOffset = 60 * SettingsS.Instance.UIScale;
            previousOffset = scrollingOffset;
            avatarDisplayedInLastBox = true;
        }
        scrolledDistance += scrollingOffset;
        scrollField.transform.position = new Vector3(scrollField.transform.position.x, scrolledDistance);

        currentDialogMessage.transform.SetParent(scrollField.transform, false);
        currentDialogMessage.transform.position = new Vector3(currentDialogMessage.transform.position.x, 150);

        currentDialogMessage.CharacterPortrait.sprite = dialogSequence.Contents[currentSequenceIndex].Speaker.Portrait;
        
        timer = 1 / SettingsS.Instance.TextboxSpeed * dialogSequence.Contents[currentSequenceIndex].TextSpeedMultiplier;

        previousAlignment = currentAlignment;
        previousName = currentName;
    }

    /// <summary>Creates a new dialog message</summary>
    /// <returns>The new dialog message with the correct alignment</returns>
    private DialogMessage InstanciateDialogMessage()
    {
        currentAlignment = dialogSequence.Contents[currentSequenceIndex].positionedLeft;
        currentName = dialogSequence.Contents[currentSequenceIndex].Speaker.SpeakerName.GetLocalizedString();
        if (dialogSequence.Contents[currentSequenceIndex].positionedLeft)
        {
            return Instantiate(leftMessagePrefab).GetComponent<DialogMessage>();
        }
        return Instantiate(rightMessagePrefab).GetComponent<DialogMessage>();
    }

    /// <summary>
    /// Fills the current text message with visible text
    /// </summary>
    private void FillCurrentBox()
    {
        currentDialogMessage.FillContent(ref timer, buttonPrompt, controls);
    }

    /// <summary>Takes the input from the game and either fills the message completely if it isn't completed yet or loads the next message</summary>
    /// <param name="ctx">Callback Context from the input action</param>
    private void OnProceedDialog(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Instance.IsPauseMenuOpen)
        {
            if (ctx.action.WasPerformedThisFrame())
            {
                clickSource.Play();
                if (currentDialogMessage.InvisibleText.Length > 0)
                    currentDialogMessage.TextSpeedMultiplier = 0;
                else
                {
                    buttonPrompt.gameObject.SetActive(false);
                    LoadNextBox();
                }
            }
        }
    }

    /// <summary>Deletes all messages from the scroll field</summary>
    private void RemoveAllMessages()
    {
        foreach (Transform message in scrollField.transform)
        {
            Destroy(message.gameObject);
        }
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
    /// If the pause menu isn't open, the current message will fill itself with text
    /// </summary>
    private void Update()
    {
        if (!GameManager.Instance.IsPauseMenuOpen)
        {
            if (currentDialogMessage != null) FillCurrentBox();
        }
    }

    /// <summary>
    /// Initializes the controls and the initial scroll position
    /// </summary>
    private void Awake()
    {
        controls = new();

        controls.UI.ProceedDialog.performed += ctx => OnProceedDialog(ctx);

        originalScrollPosition = scrollField.transform.position;
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
