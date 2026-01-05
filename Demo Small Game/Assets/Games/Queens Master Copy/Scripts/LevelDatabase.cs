using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "QueensMaster/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    public List<LevelInput> seeds;
}
[System.Serializable]
public class LevelInput
{
    public int levelNumber;
    public int lRow;
    public int lCol;
    public LevelType levelType;
}

public enum LevelType
{
    Easy,
    Medium,
    Hard,
    Expert,
    special,
    superSpecial,
}