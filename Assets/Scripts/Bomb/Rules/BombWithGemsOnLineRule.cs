using BoardLogic;
using System.Collections.Generic;
using UnityEngine;

namespace Bomb.Rules
{
    class BombWithGemsOnLineRule : IBombSpawnRule
    {
        private struct LineStats
        {
            public GlobalEnums.GemColor color;
            public int normals;
            public int bombs;
            public int total => normals + bombs;
            public bool ok => normals >= 2 && bombs >= 1 && total >= 3;
        }



        private readonly GameBoard gameBoard;

        public BombWithGemsOnLineRule(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
        }


        public List<BombSpawnDecision> Decide(IReadOnlyList<MatchCluster> clusters, Vector2Int? initiatorCell)
        {
            List<BombSpawnDecision> outDecisions = new List<BombSpawnDecision>(1);
            if (!initiatorCell.HasValue) return outDecisions;

            Vector2Int center = initiatorCell.Value;
            if (!gameBoard.InBounds(center)) 
                return outDecisions;

            if (TryBestBombPlusTwo(gameBoard, center, Vector2Int.right, out var bestH) |
                TryBestBombPlusTwo(gameBoard, center, Vector2Int.up, out var bestV))
            {
                LineStats best = (bestH.total >= bestV.total) ? bestH : bestV;

                outDecisions.Add(new BombSpawnDecision(center, GlobalEnums.GemType.bomb_diamond, best.color));
            }

            return outDecisions;
        }


        private static bool TryBestBombPlusTwo(GameBoard board, Vector2Int center, Vector2Int axis, out LineStats best)
        {
            best = default;

            HashSet<GlobalEnums.GemColor> colors = CollectCandidateColors(board, center, axis);
            if (colors.Count == 0) 
                return false;

            bool any = false;
            foreach (GlobalEnums.GemColor color in colors)
            {
                LineStats stats = CountHybridRun(board, center, axis, color);
                if (!stats.ok) 
                    continue;

                if (!any || stats.total > best.total)
                {
                    best = stats;
                    any = true;
                }
            }
            return any;
        }

        private static HashSet<GlobalEnums.GemColor> CollectCandidateColors(GameBoard board, Vector2Int center, Vector2Int axis)
        {
            var set = new HashSet<GlobalEnums.GemColor>();

            void scan(Vector2Int step)
            {
                var c = center;
                while (board.InBounds(c))
                {
                    var g = board.GetGem(c.x, c.y);
                    if (g == null) 
                        break;

                    if (g.type == GlobalEnums.GemType.regular)
                    {
                        set.Add(g.color);
                    }
                    else
                    {
                        if (g.type == GlobalEnums.GemType.bomb && g.color == GlobalEnums.GemColor.none) 
                            break;
                        set.Add(g.color);
                    }

                    c += step;
                }
            }

            scan(axis);
            scan(-axis);
            return set;
        }

        private static LineStats CountHybridRun(GameBoard board, Vector2Int center, Vector2Int axis, GlobalEnums.GemColor color)
        {
            var s = new LineStats { color = color, normals = 0, bombs = 0 };

            Acc(board, center, color, ref s);

            var c = center + axis;
            while (board.InBounds(c) && Acc(board, c, color, ref s)) c += axis;

            c = center - axis;
            while (board.InBounds(c) && Acc(board, c, color, ref s)) c -= axis;

            return s;
        }

        private static bool Acc(GameBoard board, Vector2Int cell, GlobalEnums.GemColor color, ref LineStats s)
        {
            var g = board.GetGem(cell.x, cell.y);
            if (g == null) 
                return false;

            if (g.type == GlobalEnums.GemType.regular)
            {
                if (g.color != color) 
                    return false;
                s.normals++;
                return true;
            }

            if (g.type == GlobalEnums.GemType.bomb && g.color == GlobalEnums.GemColor.none) 
                return false;
            if (g.color != color) 
                return false;

            s.bombs++;
            return true;
        }

    }
}
