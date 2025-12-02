using Bomb;
using Bomb.Rules;
using FillBoardLogic;
using Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardLogic
{
    class MatchResolver : IMatchResolver
    {
        private readonly GameBoard gameBoard;
        private readonly MatchDetector matchDetector;
        private readonly BoardFiller boardFiller;
        private readonly SC_GameVariables gameVariables;
        private readonly Action<SC_Gem> onGemDestroyed;
        private readonly BombActivationService bombService;
        private readonly List<IBombSpawnRule> bombSpawnRules;

        public MatchResolver(GameBoard gameBoard, MatchDetector matchDetector, BoardFiller boardFiller, SC_GameVariables gameVariables, Action<SC_Gem> onGemDestroyed)
        {
            this.gameBoard = gameBoard;
            this.matchDetector = matchDetector;
            this.boardFiller = boardFiller;
            this.gameVariables = gameVariables;
            this.onGemDestroyed = onGemDestroyed;

            bombService = new BombActivationService(gameBoard);
            bombSpawnRules = new List<IBombSpawnRule>()
            {
                new BombOnFourthRule(gameBoard),
                new BombWithGemsOnLineRule(gameBoard)
            };
        }

        public bool Scan(out MatchScanResult scanResult, Vector2Int? initiatorCell = null)
        {
            return matchDetector.FindAllMatches(out scanResult, initiatorCell);
        }

        public IEnumerator ResolveAll(Vector2Int? initiatorCell = null)
        {
            while (true)
            {
                bool hasResult = Scan(out MatchScanResult scanResult, initiatorCell);
                if (!hasResult)
                    yield break;

                yield return ResolveScan(scanResult, initiatorCell);
                initiatorCell = null;
            }
        }

        public IEnumerator ResolveStep(Vector2Int? initiatorCell = null)
        {
            bool hasResult = Scan(out MatchScanResult scanResult, initiatorCell);

            if (!hasResult)
                yield break;

            yield return ResolveScan(scanResult, initiatorCell);
        }

        private IEnumerator ResolveScan(MatchScanResult scan, Vector2Int? initiatorCell)
        {
            var all = new List<MatchLine>();
            if (scan.Triples != null)
                all.AddRange(scan.Triples);
            if (scan.FoursOrMore != null)
                all.AddRange(scan.FoursOrMore);

            List<MatchCluster> clusters = MatchClustering.BuildClusters(all);

            HashSet<Vector2Int> matchCells = new HashSet<Vector2Int>();

            for (int i = 0; i < clusters.Count; ++i)
            {
                foreach (Vector2Int cell in clusters[i].Cells)
                {
                    matchCells.Add(cell);
                }
            }



            List<BombSpawnDecision> decisions = new List<BombSpawnDecision>();
            for (int i = 0; i < bombSpawnRules.Count; ++i)
            {
                decisions.AddRange(bombSpawnRules[i].Decide(clusters, initiatorCell));
            }

            HashSet<Vector2Int> initBombs = new HashSet<Vector2Int>();

            foreach (Vector2Int cell in matchCells)
            {
                SC_Gem gem = gameBoard.GetGem(cell.x, cell.y);
                if (gem == null || gem.type == GlobalEnums.GemType.regular)
                    continue;

                if (!BombMatchUtils.IsBombInLineMatch(gameBoard, cell))
                    continue;

                initBombs.Add(cell);
            }

            var adjacentBombs = FindAdjacentBombsToLines(clusters, includeDiagonals: false);
            foreach (var bc in adjacentBombs)
                initBombs.Add(bc);

            List<Vector2Int> bombCenters = null;
            HashSet<Vector2Int> bombTargets = new HashSet<Vector2Int>();

            if (initBombs.Count > 0)
            {
                BombActivationResult reactionResult = bombService.BuildReactionChain(initBombs);
                for (int i = 0; i < reactionResult.targets.Count; ++i)
                {
                    bombTargets.Add(reactionResult.targets[i]);
                }
                bombCenters = reactionResult.bombCenters;

                for (int i = 0; i < bombCenters.Count; ++i)
                {
                    bombTargets.Remove(bombCenters[i]);
                }
            }

            HashSet<Vector2Int> matchOnly = new HashSet<Vector2Int>(matchCells);
            if (bombCenters != null)
            {
                foreach (Vector2Int cell in bombCenters)
                    matchOnly.Remove(cell);
            }
            foreach (Vector2Int cell in matchCells)
                bombTargets.Remove(cell);

            ClearCellsImmediate(matchOnly);

            if (gameVariables.targetsDelay > 0 && bombTargets.Count > 0)
                yield return new WaitForSeconds(gameVariables.targetsDelay);

            ClearCellsImmediate(bombTargets, true);

            if (bombCenters != null && bombCenters.Count > 0)
            {
                if (gameVariables.bombsDelay > 0)
                    yield return new WaitForSeconds(gameVariables.bombsDelay);

                ClearCellsImmediate(bombCenters, true);
            }

            for (int i = 0; i < decisions.Count; ++i)
            {
                boardFiller.PlaceBomb(decisions[i]);
            }

            yield return boardFiller.RefillBoardCor();
        }

        private void ClearCellsImmediate(IEnumerable<Vector2Int> cells, bool notify = false)
        {
            foreach (Vector2Int c in cells)
            {
                SC_Gem gem = gameBoard.GetGem(c.x, c.y);
                if (gem == null)
                    continue;

                if (notify)
                    onGemDestroyed?.Invoke(gem);

                PoolObject destroyEffectInstance = gem.destroyEffect.Pool<PoolObject>();
                destroyEffectInstance.SetPositionAndRotation(new Vector2(c.x, c.y), Quaternion.identity);

                gem.Decommission();
                gameBoard.SetGem(c.x, c.y, null);
            }
        }






        private static readonly Vector2Int[] Neigh4 =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        private static readonly Vector2Int[] Neigh8 =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
        };

        private HashSet<Vector2Int> FindAdjacentBombsToLines(IReadOnlyList<MatchCluster> clusters, bool includeDiagonals = false)
        {
            var result = new HashSet<Vector2Int>();
            var dirs = includeDiagonals ? Neigh8 : Neigh4;

            foreach (var cl in clusters)
            {
                foreach (var line in cl.Lines)
                {
                    var cells = line.CellsSorted;
                    if (cells == null || cells.Count == 0) continue;

                    for (int i = 0; i < cells.Count; i++)
                    {
                        var c = cells[i];

                        for (int d = 0; d < dirs.Length; d++)
                        {
                            var n = c + dirs[d];
                            if (!gameBoard.InBounds(n)) continue;

                            var g = gameBoard.GetGem(n.x, n.y);
                            if (g == null || g.type == GlobalEnums.GemType.regular) continue; // нас интересуют только бомбы

                            result.Add(n);
                        }
                    }
                }
            }

            return result;
        }

    }
}
