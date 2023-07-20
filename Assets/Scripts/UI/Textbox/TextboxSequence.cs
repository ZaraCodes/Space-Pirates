using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Textbox Sequence", menuName = "Textbox/Textbox Sequence")]
public class TextboxSequence : ScriptableObject
{
    public string ID;
    public TextboxContent[] Contents;
    public bool Repeat;
}