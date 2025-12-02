using System.Collections.Generic;
using UnityEngine;

namespace Bomb
{
    class BombActivationService
    {
        private readonly GameBoard gameBoard;

        public BombActivationService(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        public BombActivationResult BuildReactionChain(HashSet<Vector2Int> initialBombCells)
        {
            BombActivationResult targetCells = new BombActivationResult();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            HashSet<Vector2Int> tset = new HashSet<Vector2Int>();

            foreach(Vector2Int bombCell in initialBombCells)
            {
                if (!gameBoard.InBounds(bombCell))
                    continue;

                queue.Enqueue(bombCell);
            }

            while(queue.Count > 0)
            {
                Vector2Int center = queue.Dequeue();

                if (!visited.Add(center))
                    continue;

                SC_Gem gem = gameBoard.GetGem(center.x, center.y);
                if (gem.type == GlobalEnums.GemType.regular)
                    continue;

                List<Vector2Int> explosionArea = ComputeBombArea(gem);
                targetCells.bombCenters.Add(center);

                for(int i = 0; i < explosionArea.Count; ++i)
                {
                    Vector2Int cell = explosionArea[i];
                    if (cell == center || !gameBoard.InBounds(cell))
                        continue;

                    if (tset.Add(cell))
                        targetCells.targets.Add(cell);

                    if (visited.Contains(cell))
                        continue;

                    SC_Gem targetGem = gameBoard.GetGem(cell.x, cell.y);
                    if (targetGem.type == GlobalEnums.GemType.regular)
                        continue;

                    queue.Enqueue(cell);
                }
            }

            Deduplicate(targetCells.bombCenters);
            Deduplicate(targetCells.targets);

            return targetCells;
        }

        private List<Vector2Int> ComputeBombArea(SC_Gem gem)
        {
            if (gem.type == GlobalEnums.GemType.bomb)
                return GetRegularBombArea(gem.posIndex, gem.blastSize);

            return GetDiamondBombArea(gem.posIndex, gem.blastSize);
        }
            
        private List<Vector2Int> GetRegularBombArea(Vector2Int center, int radius)
        {
            int itemsCount = (2 * radius + 1) * (2 * radius + 1);
            List<Vector2Int> targets = new List<Vector2Int>(itemsCount);
            for(int dy = -radius; dy <= radius; ++dy)
            {
                for(int dx = -radius; dx <= radius; ++dx)
                {
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) > radius)
                        continue;

                    targets.Add(new Vector2Int(center.x + dx, center.y + dy));
                }
            }

            return targets;
        }

        private List<Vector2Int> GetDiamondBombArea(Vector2Int center, int radius)
        {
            int itemsCount = 2 * radius * (radius + 1) + 1;
            List<Vector2Int> targets = new List<Vector2Int>(itemsCount);
            for(int dy = -radius; dy <= radius; ++dy)
            {
                for(int dx = -radius; dx <= radius; ++dx)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) > radius)
                        continue;

                    targets.Add(new Vector2Int(center.x + dx, center.y + dy));
                }
            }

            return targets;
        }

        private void Deduplicate(List<Vector2Int> source)
        {
            if (source.Count < 2)
                return;

            HashSet<Vector2Int> set = new HashSet<Vector2Int>(source);
            if (set.Count == source.Count)
                return;

            source.Clear();
            source.AddRange(set);
        }
    }
}
