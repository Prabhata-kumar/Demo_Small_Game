using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class QueensGridCreator : MonoBehaviour
{
    public static QueensGridCreator Instance;

    [Header("Grid Settings")]
    public int gridSize = 8; // Max boundary
    public GameObject tilePrefab;
    public float padding = 5f;
    public RectTransform gridContainer;
    public bool isAuto = false;

    [Header("Level Data")]
    public LevelDatabase levelDatabase;
    public TMPro.TextMeshProUGUI levelText;
    public int index = 15; // Current index in the Database lis

    public int level = 0;
    public LevelType levelType;
    public int getRow, getCol;

    private GridLayoutGroup layout;
    private int[,] regionMap;
    private bool[,] crownMap;
    private DragSelection[,] allTileScripts;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        layout = GetComponent<GridLayoutGroup>();
        yield return new WaitForEndOfFrame();

        // Load the first level from your ScriptableObject list
        LoadLevelFromDatabase(index);
    }

    public void LevelUP()
    {
        index++;
        if (index < levelDatabase.seeds.Count)
        {
            LoadLevelFromDatabase(index);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }

    public void LoadLevelFromDatabase(int targetIndex)
    {
        if (levelDatabase == null || levelDatabase.seeds.Count <= targetIndex) return;

        // 1. Get Data from ScriptableObject
        LevelInput currentLevelData = levelDatabase.seeds[targetIndex];
        level = currentLevelData.levelNumber;
        levelType = currentLevelData.levelType;
        getRow = currentLevelData.lRow;
        getCol = currentLevelData.lCol;

        // 2. Lock Randomness using the level number as a Seed
        Random.InitState(level);

        // 3. Setup Logic Maps
        regionMap = new int[getRow, getCol];
        crownMap = new bool[getRow, getCol];

        for (int r = 0; r < getRow; r++)
            for (int c = 0; c < getCol; c++)
                regionMap[r, c] = -1;

        // 4. Run Generation Logic
        PlaceCrownsBacktracking(0);
        GenerateLevelTypeCast(levelType);

        // 5. Visual Construction
        BuildGrid();

        // 6. UI Update
        if (levelText != null) levelText.text = "Level " + level.ToString();
        GamePlayManager.Instance.WinScore = getRow;
    }

    void GenerateLevelTypeCast(LevelType nameTag)
    {
        switch (nameTag)
        {
            case LevelType.Medium:
                GenerateMediumRegions();
                break;
            case LevelType.Hard:
                GenerateHardRegions(); // Your existing complex logic
                break;
            case LevelType.Expert:
                GenerateExpertRegions();
                break;
            case LevelType.special:
                GenerateMediumRegions();
                break;
            case LevelType.superSpecial:
                GenerateHardRegions();
                break;
            default:
                GenerateEasyRegions();
                break;
        }
    }

    void GenerateHardRegions()
    {
        List<Vector2Int> frontier = new List<Vector2Int>();
        int regionID = 0;

        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                if (crownMap[r, c])
                {
                    regionMap[r, c] = regionID++;
                    AddNeighborsToFrontier(r, c, frontier);
                }
            }
        }

        while (frontier.Count > 0)
        {
            int randomIndex = Random.Range(0, frontier.Count);
            Vector2Int cell = frontier[randomIndex];
            frontier.RemoveAt(randomIndex);

            if (regionMap[cell.x, cell.y] == -1)
            {
                List<int> possibleColors = GetNeighborColors(cell.x, cell.y);
                if (possibleColors.Count > 0)
                {
                    regionMap[cell.x, cell.y] = possibleColors[Random.Range(0, possibleColors.Count)];
                    AddNeighborsToFrontier(cell.x, cell.y, frontier);
                }
            }
        }
    }

    void GenerateEasyRegions()
    {
        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                if (regionMap[r, c] == -1)
                {
                    // Simply find the nearest Crown and take its ID
                    regionMap[r, c] = FindNearestRegion(r, c);
                }
            }
        }
    }

    // ---------------------------------------------------------
    // MEDIUM: Standard expansion. Similar to Hard, but we 
    // pick the FIRST neighbor found instead of a random one.
    // ---------------------------------------------------------
    void GenerateMediumRegions()
    {
        List<Vector2Int> frontier = new List<Vector2Int>();
        SetupInitialRegions(frontier);

        while (frontier.Count > 0)
        {
            // For Medium, we take from the start (BFS style) 
            // which creates more "round" and predictable shapes.
            Vector2Int cell = frontier[0];
            frontier.RemoveAt(0);

            if (regionMap[cell.x, cell.y] == -1)
            {
                List<int> neighbors = GetNeighborColors(cell.x, cell.y);
                if (neighbors.Count > 0)
                {
                    regionMap[cell.x, cell.y] = neighbors[0];
                    AddNeighborsToFrontier(cell.x, cell.y, frontier);
                }
            }
        }
    }

    // ---------------------------------------------------------
    // EXPERT: Very complex. We jump around the frontier 
    // and prioritize the color with the LEAST tiles.
    // ---------------------------------------------------------
    void GenerateExpertRegions()
    {
        List<Vector2Int> frontier = new List<Vector2Int>();
        SetupInitialRegions(frontier);

        while (frontier.Count > 0)
        {
            // Randomly pick to create "snaking" patterns
            int randIdx = Random.Range(0, frontier.Count);
            Vector2Int cell = frontier[randIdx];
            frontier.RemoveAt(randIdx);

            if (regionMap[cell.x, cell.y] == -1)
            {
                List<int> colors = GetNeighborColors(cell.x, cell.y);
                // Expert logic: pick a neighbor but with a 30% chance 
                // to pick a different one to create more "interlocking" fingers
                regionMap[cell.x, cell.y] = colors[Random.Range(0, colors.Count)];
                AddNeighborsToFrontier(cell.x, cell.y, frontier);
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
            // Check boundaries against getRow and getCol
            if (nr >= 0 && nr < getRow && nc >= 0 && nc < getCol && regionMap[nr, nc] == -1)
            {
                Vector2Int pos = new Vector2Int(nr, nc);
                if (!frontier.Contains(pos)) frontier.Add(pos);
            }
        }
    }

    void SetupInitialRegions(List<Vector2Int> frontier)
    {
        int regionID = 0;
        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                if (crownMap[r, c])
                {
                    regionMap[r, c] = regionID++;
                    AddNeighborsToFrontier(r, c, frontier);
                }
            }
        }
    }

    int FindNearestRegion(int r, int c)
    {
        float minDist = float.MaxValue;
        int bestID = 0;
        int currentID = 0;
        for (int i = 0; i < getRow; i++)
        {
            for (int j = 0; j < getCol; j++)
            {
                if (crownMap[i, j])
                {
                    float d = Vector2.Distance(new Vector2(r, c), new Vector2(i, j));
                    if (d < minDist) { minDist = d; bestID = currentID; }
                    currentID++;
                }
            }
        }
        return bestID;
    }

    List<int> GetNeighborColors(int r, int c)
    {
        List<int> colors = new List<int>();
        int[] dr = { 0, 0, 1, -1 };
        int[] dc = { 1, -1, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            int nr = r + dr[i], nc = c + dc[i];
            if (nr >= 0 && nr < getRow && nc >= 0 && nc < getCol && regionMap[nr, nc] != -1)
                colors.Add(regionMap[nr, nc]);
        }
        return colors;
    }

    bool PlaceCrownsBacktracking(int row)
    {
        if (row >= getRow) return true;
        List<int> cols = new List<int>();
        for (int i = 0; i < getCol; i++) cols.Add(i);

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
        for (int i = 0; i < getRow; i++)
        {
            for (int j = 0; j < getCol; j++)
            {
                if (crownMap[i, j])
                {
                    if (j == c || i == r) return false;
                    if (Mathf.Abs(i - r) <= 1 && Mathf.Abs(j - c) <= 1) return false;
                }
            }
        }
        return true;
    }

    public void BuildGrid()
    {
       
        foreach (Transform child in transform) { Destroy(child.gameObject); }
        GamePlayManager.Instance.crownSelections.Clear();

        allTileScripts = new DragSelection[getRow, getCol];

        float totalWidth = gridContainer.rect.width;
        // Use the larger dimension to calculate cell size so it fits the box
        int maxDim = Mathf.Max(getRow, getCol);
        float cellSize = (totalWidth - (padding * (maxDim - 1))) / maxDim;

        layout.cellSize = new Vector2(cellSize, cellSize);
        layout.spacing = new Vector2(padding, padding);
        layout.constraintCount = getCol;

        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                GameObject newTile = Instantiate(tilePrefab, transform);
                newTile.name = $"Tile_{r}_{c}";

                DragSelection tileScript = newTile.GetComponent<DragSelection>();
                tileScript.row = r;
                tileScript.colomn = c;

                allTileScripts[r, c] = tileScript;

                int id = regionMap[r, c];
                if (id != -1 && GamePlayManager.Instance.imageDatas.Count > 0)
                {
                    ImageData data = GamePlayManager.Instance.imageDatas[id % GamePlayManager.Instance.imageDatas.Count];
                    newTile.GetComponent<Image>().sprite = data.colorTile;
                    tileScript.crownSprite.sprite = data.crowTile;
                }

                tileScript.isCrown = crownMap[r, c];
                if (tileScript.isCrown)
                {
                    GamePlayManager.Instance.crownSelections.Add(tileScript);
                }
            }
        }

        Debug.Log("Build GEid Done");
    }

    public void SingedUPtheGrides(int crownR, int crownC)
    {
        if (!isAuto) return;
        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                if (r == crownR && c == crownC) continue;

                bool sameRow = (r == crownR);
                bool sameCol = (c == crownC);
                bool isNeighbor = Mathf.Abs(r - crownR) <= 1 && Mathf.Abs(c - crownC) <= 1;

                if (sameRow || sameCol || isNeighbor)
                {
                    allTileScripts[r, c].AutoFillCross();
                }
            }
        }
    }
}