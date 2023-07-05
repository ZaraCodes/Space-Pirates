using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Button Icon Map", menuName = "Button Icon Map", order = 0)]
public class ButtonIconMap : ScriptableObject
{
    public string[] spriteKeys;
    public Sprite[] sprites;
}
