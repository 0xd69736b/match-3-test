using BoardLogic;
using FillBoardLogic.Data;
using FillBoardLogic.Spawning;
using System.Collections;
using UnityEngine;

namespace FillBoardLogic
{

    public abstract class SequentialFillStrategy : IBoardFillStrategy
    {
        private const string CONFIG_PATH = "AnimationsData/SequentialData";

        protected readonly GameBoard gameBoard;
        private readonly GemMover gemMover;
        protected readonly BoardGemOrchestrator orchestrator;
        protected readonly SC_GameLogic gameLogic;
        protected float duration;
        protected AnimationCurve curve;
        protected bool addToGameBoard;

        public SequentialFillStrategy(GameBoard gameBoard, IGemSpawner gemSpawner, GemMover gemMover, SC_GameLogic gameLogic)
        {
            this.gameBoard = gameBoard;
            this.gemMover = gemMover;
            this.orchestrator = new BoardGemOrchestrator(gemSpawner, gemMover);
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
                for (int y = 0; y < gameBoard.Width; ++y)
                {
                    yield return CallSpawn(x, y, parent);
                    
                }
            }
        }

        public IEnumerator ReFill(Transform parent)
        {
            for (int x = 0; x < gameBoard.Height; ++x)
            {
                for (int y = 0; y < gameBoard.Width; ++y)
                {
                    SC_Gem gem = gameBoard.GetGem(x, y);
                    if (gem != null)
                        continue;

                    yield return CallSpawn(x, y, parent);
                }
            }
        }

        protected void OnGemSpawned(SpawnedGemArgs gemArgs)
        {
            if (!addToGameBoard)
                return;

            SC_Gem gem = (SC_Gem)gemArgs.gem;
            gameBoard.SetGem(gemArgs.x, gemArgs.y, gem);
            gem.SetupGem(gameLogic, new Vector2Int(gemArgs.x, gemArgs.y), gemMover);
        }

        protected abstract IEnumerator CallSpawn(int x, int y, Transform parent);
    }

}