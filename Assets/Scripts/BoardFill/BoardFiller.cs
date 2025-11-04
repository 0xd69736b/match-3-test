using Assets.Scripts.BoardFill.GemSpawner;
using BoardLogic;
using FillBoardLogic.Data;
using FillBoardLogic.Spawning;
using System.Collections;
using UnityEngine;
using Utility;

namespace FillBoardLogic
{

    public class BoardFiller
    {
        private const string CONFIG_PATH = "AnimationsData/DefaultData";
        private readonly GameBoard gameBoard;
        private readonly CoroutineRunner coroutineRunner;
        private readonly SC_GameVariables gameVariables;
        private readonly IBoardFillStrategy backgroundStrategy;
        private readonly IBoardFillStrategy gemsFillStrategy;
        private readonly GemMover gemMover;
        private Transform gemsRoot;
        private StrategyAnimationsData defaultData;

        public BoardFiller(GameBoard gameBoard, CoroutineRunner coroutineRunner, SC_GameVariables gameVariables, SC_GameLogic gameLogic, GemMover gemMover)
        {
            this.gameBoard = gameBoard;
            this.coroutineRunner = coroutineRunner;
            this.gameVariables = gameVariables;
            this.gemMover = gemMover;
            IGemPicker gemPicker = new RandomGamePicker(gameVariables, gameBoard);
            IGemSpawner gemSpawner = new GemInstantSpawner(gemPicker, gameVariables);
            IGemPicker bgGemPicker = new BackgroundGemPicker(gameVariables);
            IGemSpawner bgGemSpawner = new BgTileSpawner(bgGemPicker);
            

            backgroundStrategy = new ParallelFillInstantStrategy(gameBoard, bgGemSpawner, gemMover, coroutineRunner, gameLogic);
            gemsFillStrategy = new SequentialFillAnimatedStrategy(gameBoard, gemSpawner, gemMover, gameLogic);
            //gemsFillStrategy = new ParallelFillInstantStrategy(gameBoard, gemSpawner, gemMover, coroutineRunner, gameLogic);
        }

        public void Init(Transform parent)
        {
            backgroundStrategy.Init();
            gemsFillStrategy.Init();
            gemsRoot = parent;
            defaultData = Resources.Load<StrategyAnimationsData>(CONFIG_PATH);
            gemMover.SetDefaultAnimationData(defaultData);
        }

        public void FillBoard()
        {
            coroutineRunner.RunCoroutine(backgroundStrategy.Fill(gemsRoot, false));
            coroutineRunner.RunCoroutine(gemsFillStrategy.Fill(gemsRoot));
        }

        public void RefillBoard()
        {
            coroutineRunner.RunCoroutine(gemsFillStrategy.ReFill(gemsRoot));
        }

        public IEnumerator RefillBoardCor()
        {
            return gemsFillStrategy.ReFill(gemsRoot);
        }

    }

}