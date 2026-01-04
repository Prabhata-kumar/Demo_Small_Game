using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GamePlayManager : MonoBehaviour
{

    public static GamePlayManager Instance;
    public int WinScore = 100;
    public int PlayerScore = 0;
    public LifeController lifeControllerManager;
    public List<DragSelection> crownSelections = new List<DragSelection>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public List<ImageData> imageDatas;
    
    public bool IsGameOver()
    {
        if(PlayerScore == WinScore) return true;
        else return false;

    }

    public void CardReavil()
    {
        PlayerScore++;
        if (IsGameOver())
        {
            Debug.Log("Game Over u win");
            lifeControllerManager.Reset();
            Invoke("ResetGame()", 1f);

        }
    }

    public void LoosTheHeart()
    {
        //scroe decrement logic can be added here if needed
        lifeControllerManager.LoseLife();
        //timere pupp logic can be added here if needed
    }

    public void ResetGame()
    {
        PlayerScore = 0;
    }

    public void GameLoos()
    {
        Debug.Log("Game Over Loos");
    }
}
[System.Serializable]
public class ImageData
{
    public Sprite colorTile, crowTile;
}

public enum TileType
{
    NotSeleted = 0, // Default
    CrownSelected = 1, // Correctly selected
}


