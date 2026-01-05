using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public LevelDatabase db;
    public QueensGridCreator gridCreator;

    public void StartLevel(int levelNumber)
    {
        // levelNumber 1 is index 0 in our list
        //int seedToUse = db.seeds[levelNumber - 1];

       /* gridCreator.GenerateBySeed(seedToUse);

        // Save that this is the current level
        PlayerPrefs.SetInt("CurrentLevelIndex", levelNumber);*/
    }
}