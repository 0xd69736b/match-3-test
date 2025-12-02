using BoardLogic;
using FillBoardLogic;
using FillBoardLogic.Data;
using FillBoardLogic.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardFill.FillStrategy
{
    public class CascadeRefillStrategy : IBoardFillStrategy
    {
        private const string CONFIG_PATH = "AnimationsData/CascadeData";

        private readonly GameBoard gameBoard;
        private readonly IGemSpawner gemSpawner;
        private readonly GemMover gemMover;
        private readonly SC_GameVariables gameVariables;
        protected float duration;
        protected AnimationCurve curve;
        protected bool addToGameBoard;
        private bool animateColumnByColumn;

        public CascadeRefillStrategy(GameBoard gameBoard, IGemSpawner gemSpawner, GemMover gemMover, SC_GameVariables gameVariables)
        {
            this.gameBoard = gameBoard;
            this.gemSpawner = gemSpawner;
            this.gemMover = gemMover;
            this.gameVariables = gameVariables;
        }


        public void Init()
        {
            StrategyAnimationsData animationData = Resources.Load<StrategyAnimationsData>(CONFIG_PATH);
            duration = animationData.duration;
            curve = animationData.curve;
            animateColumnByColumn = animationData.animateColumnByColumn;
        }

        public IEnumerator Fill(Transform parent, bool addToGameBoard = true)
        {
            bool anyFall = CollapseAllColumnsWithAnimation();
            if (anyFall)
                yield return gemMover.IdleWait;

            bool anySpawn = SpawnNewForGapsFromTop(parent);
            if (anySpawn)
                yield return gemMover.IdleWait;
        }        

        public IEnumerator ReFill(Transform parent)
        {
            bool anyFall = CollapseAllColumnsWithAnimation();
            if (anyFall)
                yield return gemMover.IdleWait;

            bool anySpawn = SpawnNewForGapsFromTop(parent);
            if (anySpawn)
                yield return gemMover.IdleWait;
        }

        private bool CollapseAllColumnsWithAnimation()
        {
            bool any = false;

            IEnumerable<int> columnsOrder = ColumnsOrder();

            foreach (int x in columnsOrder)
            {
                List<PoolObject> targets = new List<PoolObject>();
                List<Vector2Int> endPoints = new List<Vector2Int>();

                int writeY = 0;
                for (int y = 0; y < gameBoard.Height; y++)
                {
                    SC_Gem gem = gameBoard.GetGem(x, y);
                    if (!gem) continue;

                    if (y != writeY)
                    {
                        Vector2Int endpos = new Vector2Int(x, writeY);
                        any = true;
                        targets.Add(gem);
                        endPoints.Add(endpos);
                        gem.SetupGem(endpos);
                        gameBoard.SetGem(x, writeY, gem);
                        gameBoard.SetGem(x, y, null);
                    }
                    writeY++;
                }

                if (targets.Count == 0)
                    continue;

                gemMover.EnqueueMove(targets, endPoints, duration, curve);
                gemMover.WaitUntilIdle();
            }

            return any;
        }

        private bool SpawnNewForGapsFromTop(Transform parent)
        {
            bool any = false;

            IEnumerable<int> columnsOrder = ColumnsOrder();

            foreach (int x in columnsOrder)
            {
                int emptyCount = 0;
                for (int y = gameBoard.Height - 1; y >= 0; y--)
                {
                    SC_Gem gem = gameBoard.GetGem(x, y);
                    if (gem == null) 
                        emptyCount++;
                    else 
                        break;
                }
                if (emptyCount == 0) 
                    continue;

                any = true;

                List<PoolObject> targets = new List<PoolObject>();
                List<Vector2Int> endPoints = new List<Vector2Int>();

                int topY = gameBoard.Height - emptyCount;
                for (int i = 0; i < emptyCount; i++)
                {
                    int y = topY + i;

                    Vector2Int endpos = new Vector2Int(x, y);
                    Vector2Int start = endpos + Vector2Int.up * (gameVariables.dropHeight + i * gameVariables.dropHeight);

                    SC_Gem gemInst = (SC_Gem)gemSpawner.Spawn(x, y, start, parent);
                    targets.Add(gemInst);
                    endPoints.Add(endpos);
                    gemInst.SetupGem(endpos);
                    gameBoard.SetGem(x, y, gemInst);

                }

                if (targets.Count > 0)
                {
                    gemMover.EnqueueMove(targets, endPoints, duration, curve);
                    if (animateColumnByColumn)
                        gemMover.WaitUntilIdle();
                }
            }

            return any;
        }

        private IEnumerable<int> ColumnsOrder()
        {
            for (int x = 0; x < gameBoard.Width; x++)
                yield return x;
        }
    }
}
