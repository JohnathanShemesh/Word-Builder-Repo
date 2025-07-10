using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Word Game/Level Database")]
public class LevelDatabase : ScriptableObject
{
    public List<LevelData> levels;
}
