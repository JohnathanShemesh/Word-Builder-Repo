using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "NewWord", menuName = "Word Game/Word Data")]
    public class WordData : ScriptableObject
    {
        public string wordName;             // שם טקסטואלי של המילה, לצורך תצוגה או לוגיקה
    }