using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogOverlay : MonoBehaviour
{
    #region Fields
    private SpacePiratesControls controls;

    private Vector3 originalScrollPosition;

    private int currentSectionIndex;
    private int currentSequenceIndex;
    private int sequenceLength;
    private TextboxSequence dialogSequence;
    private string[] sectionStrings;

    private DialogMessage currentDialogMessage;

    private float timer;

    private bool previousAlignment;
    private bool currentAlignment;
    private string previousName;
    private string currentName;

    private float scrolledDistance;

    private bool avatarDisplayedInLastBox;

    [Header("References"), SerializeField] private GameObject scrollField;
    [Header("Prefabs"), SerializeField] private GameObject leftMessagePrefab;
    [SerializeField] private GameObject rightMessagePrefab;
    #endregion

    #region Methods
    public void LoadDialog(string sequenceName)
    {
        GameManager.Instance.IsDialogOverlayOpen = true;

        avatarDisplayedInLastBox = false;
        scrolledDistance = originalScrollPosition.y;
        //scrollField.transform.position = new Vector3(scrollField.transform.position.x, 0);

        dialogSequence = AllDialogs.Instance.GetSequence(sequenceName);

        if (dialogSequence == null)
        {
            Debug.LogWarning($"The dialog \"{sequenceName}\" does not exist!");
            return;
        }
        currentSequenceIndex = 0;
        currentSectionIndex = -1;

        sequenceLength = dialogSequence.Contents.Length;
        sectionStrings = dialogSequence.Contents[currentSequenceIndex].SequenceText.GetLocalizedString().Split("\n||\n");

        previousAlignment = !dialogSequence.Contents[currentSequenceIndex].positionedLeft;
        previousName = dialogSequence.Contents[currentSequenceIndex].Speaker.SpeakerName.GetLocalizedString() + "c";

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
                scrollingOffset = 60 * SettingsS.Instance.UIScale;
                avatarDisplayedInLastBox = false;
            }
            else scrollingOffset = (scrollingOffset + 10) * SettingsS.Instance.UIScale;
            currentDialogMessage.PortraitFrame.SetActive(false);
        }
        else
        {
            // Debug.Log($"{currentName} {previousName} {currentAlignment} {previousAlignment}");
            scrollingOffset = 60 * SettingsS.Instance.UIScale;
            avatarDisplayedInLastBox = true;
        }
        // Debug.Log(scrollingOffset);
        scrolledDistance += scrollingOffset;
        scrollField.transform.position = new Vector3(scrollField.transform.position.x, scrolledDistance);

        currentDialogMessage.transform.SetParent(scrollField.transform, false);
        currentDialogMessage.transform.position = new Vector3(currentDialogMessage.transform.position.x, 150);

        currentDialogMessage.CharacterPortrait.sprite = dialogSequence.Contents[currentSequenceIndex].Speaker.Portrait;
        
        timer = 1 / SettingsS.Instance.TextboxSpeed * dialogSequence.Contents[currentSequenceIndex].TextSpeedMultiplier;

        previousAlignment = currentAlignment;
        previousName = currentName;
    }

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

    private void FillCurrentBox()
    {
        currentDialogMessage.FillContent(ref timer);
    }

    private void OnProceedDialog(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Instance.IsPauseMenuOpen)
        {
            if (ctx.action.WasPerformedThisFrame())
            {
                if (currentDialogMessage.InvisibleText.Length > 0)
                    currentDialogMessage.TextSpeedMultiplier = 0;
                else 
                    LoadNextBox();
            }
        }
    }

    private void RemoveAllMessages()
    {
        foreach (Transform message in scrollField.transform)
        {
            Destroy(message.gameObject);
        }
    }
    #endregion

    #region Unity Stuff
    private void Update()
    {
        if (!GameManager.Instance.IsPauseMenuOpen)
        {
            if (currentDialogMessage != null) FillCurrentBox();
        }
    }

    private void Awake()
    {
        controls = new();

        controls.UI.ProceedDialog.performed += ctx => OnProceedDialog(ctx);

        originalScrollPosition = scrollField.transform.position;
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
