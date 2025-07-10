using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "NewLevel", menuName = "Word Game/Level Data")]
    public class LevelData : ScriptableObject
    {
        public WordData wordData;
        public int fakeLettersToSpawn = 3; // ברירת מחדל, אפשר לשנות בעתיד
    }

