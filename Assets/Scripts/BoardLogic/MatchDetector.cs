using System.Collections.Generic;
using UnityEngine;

namespace BoardLogic
{

    public class MatchDetector
    {
        private readonly GameBoard gameBoard;

        public MatchDetector(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        public bool FindAllMatches(out MatchScanResult matchScanResult, Vector2Int? triggerCell = null)
        {
            matchScanResult = new MatchScanResult();

            var triples = new List<MatchLine>();
            var fours = new List<MatchLine>();

            for (int y = 0; y < gameBoard.Height; y++)
            {
                int x = 0;
                while (x < gameBoard.Width)
                {
                    List<Vector2Int> run = CollectRunHorizontal(x, y, out GlobalEnums.GemColor color, out GlobalEnums.GemType type);
                    if (run.Count >= 3)
                        PushLine(run, GlobalEnums.LineOrientation.Horizontal, color, type, triggerCell, triples, fours);
                    int shift = Mathf.Max(1, run.Count);
                    x += shift;
                }
            }

            for (int x = 0; x < gameBoard.Width; x++)
            {
                int y = 0;
                while (y < gameBoard.Height)
                {
                    List<Vector2Int> run = CollectRunVertical(x, y, out GlobalEnums.GemColor color, out GlobalEnums.GemType type);
                    if (run.Count >= 3)
                        PushLine(run, GlobalEnums.LineOrientation.Horizontal, color, type, triggerCell, triples, fours);
                    int shift = Mathf.Max(1, run.Count);
                    y += shift;
                }
            }

            if (triples.Count == 0 && fours.Count == 0)
                return false;

            matchScanResult.Triples = triples;
            matchScanResult.FoursOrMore = fours;

            return true;
        }

        private List<Vector2Int> CollectRunHorizontal(int startX, int y, out GlobalEnums.GemColor color, out GlobalEnums.GemType type)
        {
            List<Vector2Int> res = new List<Vector2Int>(8);
            SC_Gem gem = gameBoard.GetGem(startX, y);

            color = GlobalEnums.GemColor.none;
            type = GlobalEnums.GemType.regular;

            if (gem == null) 
                return res;

            color = gem.color;
            type = gem.type;

            int x = startX;
            while (x < gameBoard.Width)
            {
                SC_Gem nGem = gameBoard.GetGem(x, y);
                if (nGem == null || !gem.Equals(nGem)) 
                    break;
                res.Add(new Vector2Int(x, y));
                x++;
            }
            return res;
        }

        private List<Vector2Int> CollectRunVertical(int x, int startY, out GlobalEnums.GemColor color, out GlobalEnums.GemType type)
        {
            List<Vector2Int> res = new List<Vector2Int>(8);
            SC_Gem gem = gameBoard.GetGem(x, startY);
            color = GlobalEnums.GemColor.none;
            type = GlobalEnums.GemType.regular;

            if (gem == null)
                return res;

            color = gem.color;
            type = gem.type;

            int y = startY;
            while (y < gameBoard.Height)
            {
                SC_Gem nGem = gameBoard.GetGem(x, y);
                if (nGem == null || !gem.Equals(nGem))
                    break;
                res.Add(new Vector2Int(x, y));
                y++;
            }
            return res;
        }

        private void PushLine(List<Vector2Int> run, GlobalEnums.LineOrientation orientation, GlobalEnums.GemColor color,
                              GlobalEnums.GemType type, Vector2Int? initiatorCell, List<MatchLine> triples, List<MatchLine> fours)
        {
            var line = new MatchLine
            {
                CellsSorted = run,
                Orientation = orientation,
                Color = color,
                Type = type,
                TriggerCell = ResolveTrigger(run, initiatorCell)
            };

            if (run.Count > 3) 
                fours.Add(line);
            else 
                triples.Add(line);
        }

        private Vector2Int ResolveTrigger(List<Vector2Int> run, Vector2Int? initiatorCell)
        {
            if (initiatorCell.HasValue)
            {
                foreach (Vector2Int c in run)
                    if (c == initiatorCell.Value)
                        return c;
            }

            return run.Count >= 4 ? run[3] : run[1];
        }

    }

}