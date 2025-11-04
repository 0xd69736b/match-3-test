using BoardLogic;
using FillBoardLogic.Data;
using FillBoardLogic.Spawning;
using System.Collections;
using UnityEngine;
using Utility;

namespace FillBoardLogic
{

    public abstract class ParallelFillStrategy : IBoardFillStrategy
    {
        private const string CONFIG_PATH = "AnimationsData/ParallelData";

        protected readonly GameBoard gameBoard;
        private readonly GemMover gemMover;
        protected readonly BoardGemOrchestrator orchestrator;
        protected readonly CoroutineRunner coroutineRunner;
        protected readonly SC_GameLogic gameLogic;
        protected float duration;
        protected AnimationCurve curve;
        protected bool addToGameBoard;

        public ParallelFillStrategy(GameBoard gameBoard, IGemSpawner gemSpawner, GemMover gemMover, CoroutineRunner coroutineRunner, SC_GameLogic gameLogic)
        {
            this.gameBoard = gameBoard;
            this.gemMover = gemMover;
            this.orchestrator = new BoardGemOrchestrator(gemSpawner, gemMover);
            this.coroutineRunner = coroutineRunner;
            this.gameLogic = gameLogic;
        }

        public void Init()
        {
            StrategyAnimationsData animationData = Resources.Load<StrategyAnimationsData>(CONFIG_PATH);
            duration = animationData.duration;
            curve = animationData.curve;
        }

        public IEnumerator Fill(Transform parent, bool addToGameBoard = true)
        {
            this.addToGameBoard = addToGameBoard;
            for (int x = 0; x < gameBoard.Height; ++x)
            {
                for(int y = 0; y < gameBoard.Width; ++y)
                {
                    CallSpawn(x, y, parent);
                }
            }

            yield return orchestrator.WaitUntilIdle();
        }


        public IEnumerator ReFill(Transform parent)
        {
            bool hasToSpawn = false;

            for (int x = 0; x < gameBoard.Height; ++x)
            {
                for (int y = 0; y < gameBoard.Width; ++y)
                {
                    SC_Gem gem = gameBoard.GetGem(x, y);
                    if (gem != null)
                        continue;

                    hasToSpawn = true;
                    CallSpawn(x, y, parent);
                }
            }

            if (hasToSpawn)
                yield return orchestrator.WaitUntilIdle();
        }

        protected void OnGemSpawned(SpawnedGemArgs gemArgs)
        {
            if (!addToGameBoard)
                return;

            SC_Gem gem = (SC_Gem)gemArgs.gem;
            gameBoard.SetGem(gemArgs.x, gemArgs.y, gem);
            gem.SetupGem(gameLogic, new Vector2Int(gemArgs.x, gemArgs.y), gemMover);
        }

        protected abstract void CallSpawn(int x, int y, Transform parent);
    }

}