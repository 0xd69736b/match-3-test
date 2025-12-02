using BoardLogic;
using UnityEngine;

public class GameBoard
{
    #region Variables

    private readonly BoardIndexCache gemsCache;
    private Vector2Int origin = Vector2Int.zero;
    private int cellSize = 1;

    private int height = 0;

    public int Height { get { return height; } }

    private int width = 0;
    public int Width { get { return width; } }
  
    private SC_Gem[,] allGems;

    private int score = 0;
    public int Score 
    {
        get { return score; }
        set { score = value; }
    }

    #endregion

    public GameBoard(int _Width, int _Height)
    {
        height = _Height;
        width = _Width;
        allGems = new SC_Gem[width, height];
        gemsCache = new BoardIndexCache();
    }
    public bool MatchesAt(Vector2Int _PositionToCheck, SC_Gem _GemToCheck)
    {
        if (_PositionToCheck.x > 1)
        {
            SC_Gem prev1Gem = allGems[_PositionToCheck.x - 1, _PositionToCheck.y];
            SC_Gem prev2Gem = allGems[_PositionToCheck.x - 2, _PositionToCheck.y];

            if (prev1Gem == null || prev2Gem == null)
                return false;

            if (prev1Gem.Equals(_GemToCheck) && prev2Gem.Equals(_GemToCheck))
                return true;
        }

        if (_PositionToCheck.y > 1)
        {
            SC_Gem prev1Gem = allGems[_PositionToCheck.x, _PositionToCheck.y - 1];
            SC_Gem prev2Gem = allGems[_PositionToCheck.x, _PositionToCheck.y - 2];

            if (prev1Gem == null || prev2Gem == null)
                return false;

            if (prev1Gem.Equals(_GemToCheck) && prev2Gem.Equals(_GemToCheck))
                return true;
        }

        return false;
    }

    public bool InBounds(Vector2Int cell)
    {
        return InBounds(cell.x, cell.y);
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y - origin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public void SetGem(int _X, int _Y, SC_Gem _Gem)
    {
        allGems[_X, _Y] = _Gem;
        GameObject gemGO = _Gem == null ? null : _Gem.GO;
        gemsCache.OnSet(_X, _Y, gemGO);
    }

    public SC_Gem GetGem(int _X,int _Y)
    {
       return allGems[_X, _Y];
    }

    public bool TryGetGemCell(GameObject gemGO, out Vector2Int cell)
    {
        return gemsCache.TryGetCell(gemGO, out cell);
    }

}

