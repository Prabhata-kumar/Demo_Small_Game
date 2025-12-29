using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GamePlayManager : MonoBehaviour
{

    public static GamePlayManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public List<ImageData> imageDatas;
    

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


