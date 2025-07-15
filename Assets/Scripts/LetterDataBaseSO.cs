using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Letters", menuName = "Word Game/CreateLetterData")]

public class LetterDataBaseSO : ScriptableObject
{
    public List<LetterDataSO> letterSOs = new List<LetterDataSO>();

    public LetterDataSO GetLetter(string letterName)
    {
        var letter = letterSOs.FirstOrDefault(l => l.letterName.ToUpper() == letterName.ToUpper());
        //returns a letterSO 
        // goes over letterSO list and returns the letterSO 
        //use linq 

        if (letter == null)
        {
            Debug.LogWarning("Letter not found: " + letterName);
        }

        return letter;
    }
}
