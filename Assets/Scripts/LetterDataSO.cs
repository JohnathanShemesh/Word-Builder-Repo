using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Letters", menuName = "Word Game/CreateLetter")]

public class LetterDataSO : ScriptableObject
{
    public string letterName;
    public Sprite upperCase;
    public Sprite lowerCase;
}
