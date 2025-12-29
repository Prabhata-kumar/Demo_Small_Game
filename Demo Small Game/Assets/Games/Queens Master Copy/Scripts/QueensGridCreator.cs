using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueensGridCreator : MonoBehaviour
{
    public int gridSize = 8;
    public GameObject tilePrefab;
    public float padding = 5f;
    public RectTransform gridContainer;
    private GridLayoutGroup layout;

    private int[,] regionMap;
    private bool[,] crownMap;

    private IEnumerator Start()
    {
        layout = GetComponent<GridLayoutGroup>();
        yield return new WaitForEndOfFrame();

        GenerateLevel();
        BuildGrid();
    }

    void GenerateLevel()
    {
        regionMap = new int[gridSize, gridSize];
        crownMap = new bool[gridSize, gridSize];

        // Initialize maps
        for (int r = 0; r < gridSize; r++)
            for (int c = 0; c < gridSize; c++)
                regionMap[r, c] = -1;

        // 1. Place Crowns with the strict Queens Rules
        PlaceCrownsBacktracking(0);

        // 2. Generate HARD/Dynamic Regions
        GenerateHardRegions();
    }

    // Logic to make shapes "snake" and interlock
    void GenerateHardRegions()
    {
        // A list of cells that are on the "edge" of a color and can expand
        List<Vector2Int> frontier = new List<Vector2Int>();

        // Start seeds at crown positions
        int regionID = 0;
        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize; c++)
            {
                if (crownMap[r, c])
                {
                    regionMap[r, c] = regionID++;
                    AddNeighborsToFrontier(r, c, frontier);
                }
            }
        }

        // Expand randomly (this creates the snaking, complex shapes)
        while (frontier.Count > 0)
        {
            // Pick a RANDOM cell from the frontier to expand
            int randomIndex = Random.Range(0, frontier.Count);
            Vector2Int cell = frontier[randomIndex];
            frontier.RemoveAt(randomIndex);

            if (regionMap[cell.x, cell.y] == -1)
            {
                // Find which colored neighbor this cell should join
                List<int> possibleColors = GetNeighborColors(cell.x, cell.y);
                if (possibleColors.Count > 0)
                {
                    regionMap[cell.x, cell.y] = possibleColors[Random.Range(0, possibleColors.Count)];
                    // Add this new cell's neighbors to the frontier to keep growing
                    AddNeighborsToFrontier(cell.x, cell.y, frontier);
                }
            }
        }
    }

    void AddNeighborsToFrontier(int r, int c, List<Vector2Int> frontier)
    {
        int[] dr = { 0, 0, 1, -1 };
        int[] dc = { 1, -1, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            int nr = r + dr[i];
            int nc = c + dc[i];
            if (nr >= 0 && nr < gridSize && nc >= 0 && nc < gridSize && regionMap[nr, nc] == -1)
            {
                Vector2Int pos = new Vector2Int(nr, nc);
                if (!frontier.Contains(pos)) frontier.Add(pos);
            }
        }
    }

    List<int> GetNeighborColors(int r, int c)
    {
        List<int> colors = new List<int>();
        int[] dr = { 0, 0, 1, -1 };
        int[] dc = { 1, -1, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            int nr = r + dr[i], nc = c + dc[i];
            if (nr >= 0 && nr < gridSize && nc >= 0 && nc < gridSize && regionMap[nr, nc] != -1)
                colors.Add(regionMap[nr, nc]);
        }
        return colors;
    }

    // --- Backtracking & BuildGrid remain largely the same, but now called after Hard Gen ---
    bool PlaceCrownsBacktracking(int row)
    {
        if (row >= gridSize) return true;
        List<int> cols = new List<int>();
        for (int i = 0; i < gridSize; i++) cols.Add(i);
        // Shuffle columns for true randomness
        for (int i = 0; i < cols.Count; i++)
        {
            int temp = cols[i]; int rand = Random.Range(i, cols.Count);
            cols[i] = cols[rand]; cols[rand] = temp;
        }

        foreach (int col in cols)
        {
            if (IsSafe(row, col))
            {
                crownMap[row, col] = true;
                if (PlaceCrownsBacktracking(row + 1)) return true;
                crownMap[row, col] = false;
            }
        }
        return false;
    }

    bool IsSafe(int r, int c)
    {
        for (int i = 0; i < gridSize; i++)
            for (int j = 0; j < gridSize; j++)
                if (crownMap[i, j])
                {
                    if (j == c || i == r) return false; // Row/Col check
                    if (Mathf.Abs(i - r) <= 1 && Mathf.Abs(j - c) <= 1) return false; // Adjacent/Diagonal check
                }
        return true;
    }

    public void BuildGrid()
    {
        foreach (Transform child in transform) { Destroy(child.gameObject); }
        float totalWidth = gridContainer.rect.width;
        float cellSize = (totalWidth - (padding * (gridSize - 1))) / gridSize;
        layout.cellSize = new Vector2(cellSize, cellSize);
        layout.spacing = new Vector2(padding, padding);
        layout.constraintCount = gridSize;

        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize; c++)
            {
                GameObject newTile = Instantiate(tilePrefab, transform);
                DragSelection tileScript = newTile.GetComponent<DragSelection>();
                int id = regionMap[r, c];
                ImageData data = GamePlayManager.Instance.imageDatas[id % GamePlayManager.Instance.imageDatas.Count];
                newTile.GetComponent<Image>().sprite = data.colorTile;
                tileScript.crownSprite.sprite = data.crowTile;
                tileScript.isCrown = crownMap[r, c];
            }
        }
    }
}