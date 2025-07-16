using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "NewWord", menuName = "Word Game/Word Data")]
    public class WordData : ScriptableObject
    {
        public string wordName;
    public int fakeLettersToSpawn = 3;
}