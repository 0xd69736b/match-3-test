using BoardLogic;
using System.Collections.Generic;
using UnityEngine;

namespace Bomb.Rules
{
    public struct BombSpawnDecision
    {
        public readonly Vector2Int cell;
        public readonly GlobalEnums.GemType gemType;
        public readonly GlobalEnums.GemColor gemColor;

        public BombSpawnDecision(Vector2Int cell, GlobalEnums.GemType gemType, GlobalEnums.GemColor gemColor)
        {
            this.cell = cell;
            this.gemType = gemType;
            this.gemColor = gemColor;
        }
    }

    public class BombOnFourthRule : IBombSpawnRule
    {
        private readonly GameBoard gameBoard;

        public BombOnFourthRule(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        private static int ResolveIndex(IList<Vector2Int> sorted, Vector2Int? trigger)
        {
            if (trigger.HasValue)
            {
                for (int i = 0; i < sorted.Count; i++)
                {
                    if (sorted[i] == trigger.Value)
                        return i;
                }
            }

            return 3;
        }

        public List<BombSpawnDecision> Decide(IReadOnlyList<MatchCluster> clusters, Vector2Int? triggerCell)
        {
            List<BombSpawnDecision> decisions = new List<BombSpawnDecision>();

            for(int i = 0; i < clusters.Count; ++i)
            {
                MatchCluster cluster = clusters[i];

                if (!cluster.HasFourPlus)
                    continue;

                MatchLine four = cluster.Lines.Find(l => l.CellsSorted.Count >= 4);
                if (four == null || four.CellsSorted == null || four.CellsSorted.Count < 4) 
                    continue;

                int idx = ResolveIndex(four.CellsSorted, triggerCell);
                Vector2Int cell = four.CellsSorted[idx];
                SC_Gem gem = gameBoard.GetGem(cell.x, cell.y);
                if (gem == null)
                    continue;

                BombSpawnDecision decision = new BombSpawnDecision(cell, GlobalEnums.GemType.bomb_diamond, gem.color);

                decisions.Add(decision);
            }
            return decisions;
        }
    }
}
