using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Added for easier list searching
using UnityEngine;

public class TikTackToManager : MonoBehaviour
{
    public static TikTackToManager Instance;

    public GameObject tilePrefab;
    public Transform tilesParent;
    public Sprite playerXSprite, playerOSprite;
    public TMPro.TextMeshProUGUI infoText;

    private List<TicTackToTile> allTiles = new List<TicTackToTile>();
    private List<TicTackToTile> playerChooseTile = new List<TicTackToTile>();
    private List<TicTackToTile> computerChooseTile = new List<TicTackToTile>();

    // Winning combinations: Rows, Columns, Diagonals
    private readonly int[][] winPatterns = new int[][]
    {
        new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8}, // Rows
        new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8}, // Columns
        new int[] {0, 4, 8}, new int[] {2, 4, 6}              // Diagonals
    };

    public static Action<bool> TileInteraction;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CreateTiles();
        infoText.text = "";
    }

    private void CreateTiles()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject tileObj = Instantiate(tilePrefab, tilesParent);
            TicTackToTile tile = tileObj.GetComponent<TicTackToTile>();
            tile.index = i;
            allTiles.Add(tile);
        }
    }

    public Sprite OnTileSelected(int index, bool computer = false)
    {
        if (computer)
        {
            computerChooseTile.Add(allTiles[index]);
            return playerOSprite;
        }
        else
        {
            playerChooseTile.Add(allTiles[index]);
            StartCoroutine(CounterPully());
            return playerXSprite;
        }
    }

    public IEnumerator CounterPully()
    {
        // 1. Disable input while computer thinks
        TileController(false);

        // 2. Check if Player Won
        if (CheckWin(playerChooseTile))
        {
            Debug.Log("PLAYER WON!");
            yield break; // Stop game logic
        }

        // 3. Endless Logic: Remove oldest player tile if they have more than 3
        if (playerChooseTile.Count > 3)
        {
            playerChooseTile[0].ResetTile();
            playerChooseTile.RemoveAt(0);
        }

        yield return new WaitForSeconds(1f);

        // 4. Computer Move
        ComputerResponse();

        // 5. Check if Computer Won
        if (CheckWin(computerChooseTile))
        {
            Debug.Log("COMPUTER WON!");
            yield break;
        }

        // 6. Endless Logic: Remove oldest computer tile if they have more than 3
        if (computerChooseTile.Count > 3)
        {
            computerChooseTile[0].ResetTile();
            computerChooseTile.RemoveAt(0);
        }

        // 7. Re-enable interaction
        yield return new WaitForSeconds(1f);
        TileController(true);
    }

    public void ComputerResponse()
    {
        int targetIndex = -1;

        // 1. MUST WIN: If computer has 2 in a row, take the 3rd.
        targetIndex = GetWinningMove(computerChooseTile);

        // 2. MUST BLOCK: If player has 2 in a row, block them.
        if (targetIndex == -1)
            targetIndex = GetWinningMove(playerChooseTile);

        // 3. CREATE FORK: Look for a move that creates two paths to victory.
        if (targetIndex == -1)
            targetIndex = GetForkMove(computerChooseTile);

        // 4. BLOCK PLAYER FORK: Don't let the player create a double-threat.
        if (targetIndex == -1)
            targetIndex = GetForkMove(playerChooseTile);

        // 5. CENTER: Most important strategic tile.
        if (targetIndex == -1 && !allTiles[4].isSelected)
            targetIndex = 4;

        // 6. CORNERS: Prioritize indices 0, 2, 6, 8.
        if (targetIndex == -1)
        {
            int[] corners = { 0, 2, 6, 8 };
            foreach (int c in corners)
            {
                if (!allTiles[c].isSelected) { targetIndex = c; break; }
            }
        }

        // 7. SIDES: Final fallback (1, 3, 5, 7).
        if (targetIndex == -1)
        {
            List<TicTackToTile> emptyTiles = allTiles.FindAll(t => !t.isSelected);
            if (emptyTiles.Count > 0)
                targetIndex = emptyTiles[UnityEngine.Random.Range(0, emptyTiles.Count)].index;
        }

        // Execute Move
        if (targetIndex != -1)
        {
            TicTackToTile selectedTile = allTiles[targetIndex];
            selectedTile.isSelected = true;
            selectedTile.coreImage.raycastTarget = false;
            selectedTile.tileImage.sprite = OnTileSelected(selectedTile.index, true);
            selectedTile.tileImage.color = Color.white;
        }
    }

    private int GetForkMove(List<TicTackToTile> teamTiles)
    {
        List<int> currentIndices = teamTiles.Select(t => t.index).ToList();

        // Check every empty tile
        for (int i = 0; i < 9; i++)
        {
            if (allTiles[i].isSelected) continue;

            // "Simulate" placing a tile here
            int waysToWin = 0;
            List<int> simulatedIndices = new List<int>(currentIndices) { i };

            foreach (var pattern in winPatterns)
            {
                int count = 0;
                int emptyInPattern = 0;
                foreach (int pos in pattern)
                {
                    if (simulatedIndices.Contains(pos)) count++;
                    else if (!allTiles[pos].isSelected) emptyInPattern++;
                }

                // If this move creates a line where 2 are filled and 1 is free
                if (count == 2 && emptyInPattern == 1) waysToWin++;
            }

            if (waysToWin >= 2) return i; // Found a fork!
        }
        return -1;
    }

    // Helper function to find a winning or blocking move
    private int GetWinningMove(List<TicTackToTile> currentTeamTiles)
    {
        List<int> indices = currentTeamTiles.Select(t => t.index).ToList();

        foreach (var pattern in winPatterns)
        {
            int count = 0;
            int emptyIndex = -1;

            foreach (int pos in pattern)
            {
                if (indices.Contains(pos)) count++;
                else emptyIndex = pos;
            }

            // If two are filled and the third is empty, return the empty index
            if (count == 2 && emptyIndex != -1 && !allTiles[emptyIndex].isSelected)
            {
                return emptyIndex;
            }
        }
        return -1;
    }
    public bool CheckWin(List<TicTackToTile> pTiles)
    {
        // Get a list of indices currently held by the player/computer
        List<int> currentIndices = pTiles.Select(t => t.index).ToList();

        foreach (var pattern in winPatterns)
        {
            // If the player has all three indices of any win pattern
            if (pattern.All(index => currentIndices.Contains(index)))
            {
                StartCoroutine(DisplayText());
                return true;
            }
        }
        return false;
    }
    public IEnumerator DisplayText()
    {
        TileController(false);
        for (int i = 5; i > 0; i--)
        {
            infoText.text = "Restarting in " + i + "...";
            yield return new WaitForSeconds(1f);
        }
        ResetGame();
        infoText.text = "";
    }
    public void ResetGame()
    {
        playerChooseTile.Clear();
        computerChooseTile.Clear();
        foreach (var tile in allTiles)
        {
            tile.ResetTile();
        }
        TileController(true);
    }

    public void TileController(bool getValue)
    {
        TileInteraction?.Invoke(getValue);
        Debug.Log("Tile Interaction set to: " + getValue);
    }
}