using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "NewLevel", menuName = "Word Game/Level Data")]
    public class LevelData : ScriptableObject
    {
        public List<WordData> wordData = new List<WordData>();
    }

