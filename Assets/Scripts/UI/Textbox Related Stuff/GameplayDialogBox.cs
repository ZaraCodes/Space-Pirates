using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.UI;

public class GameplayDialogBox : MonoBehaviour
{
    #region Fields
    private int currentSequenceIndex;
    private int sequenceLength;

    [Header("References"), SerializeField] private TextMeshProUGUI content;
    [SerializeField] private Image characterPortrait;
    private TextboxSequence textboxSequence;
    #endregion

    #region Methods
    private void LoadDialog()
    {
        sequenceLength = textboxSequence.contents.Length;
        currentSequenceIndex = 0;
    }
    #endregion

    #region Unity Stuff

    #endregion
}
