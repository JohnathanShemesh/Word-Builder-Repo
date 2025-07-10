using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlphabetData", menuName = "Word Game/Alphabet Data")]

public class Alphabet : ScriptableObject
{
    public List<Sprite> letterSprites;
}
