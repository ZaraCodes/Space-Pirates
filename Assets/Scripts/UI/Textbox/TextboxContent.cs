using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New TextBox Content", menuName = "Textbox/Textbox Content")]
public class TextboxContent : ScriptableObject
{
    public TextboxSpeaker Speaker;
    public LocalizedString SequenceText;
    public float TextSpeedMultiplier = 1f;
    public bool positionedLeft = true;
    public UnityEvent OnContentFinished;
}