using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeController : MonoBehaviour
{
    public List<GameObject> lifeIcons;
    private int index = 0;

    private void Start()
    {
        Reset();
    }

    // Call this when the player loses a life
    public void LoseLife()
    {
        if (index < lifeIcons.Count)
        {
            // Use square brackets [] for lists and SetActive for Unity
            lifeIcons[index].SetActive(false);
            index++;
        }
        else
        {
            GamePlayManager.Instance.GameLoos();
        }
    }

    // Call this to restart the level
    public void Reset()
    {
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            // Fixed the capital letters here
            lifeIcons[i].SetActive(true);
        }
        index = 0;
    }
}