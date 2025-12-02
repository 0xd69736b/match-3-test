using Assets.Scripts.BoardFill.GemSpawner;
using BoardFill.FillStrategy;
using BoardLogic;
using Bomb;
using Bomb.Rules;
using FillBoardLogic.Data;
using FillBoardLogic.Spawning;
using System.Collections;
using System.Collections.Generic;
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
        private readonly IColoredBombPicker coloredBombPicker;
        private readonly IBombPlacer bombPlacer;
        private Transform gemsRoot;
        private StrategyAnimationsData defaultData;

        public BoardFiller(GameBoard gameBoard, CoroutineRunner coroutineRunner, SC_GameVariables gameVariables, 
                           SC_GameLogic gameLogic, GemMover gemMover)
        {
            this.gameBoard = gameBoard;
            this.coroutineRunner = coroutineRunner;
            this.gameVariables = gameVariables;
            this.gemMover = gemMover;
            IGemPicker gemPicker = new RandomGemPicker(gameVariables, gameBoard);
            IGemSpawner gemSpawner = new GemInstantSpawner(gemPicker, gameVariables, gameBoard);
            IGemPicker bgGemPicker = new BackgroundGemPicker(gameVariables);
            IGemSpawner bgGemSpawner = new BgTileSpawner(bgGemPicker);
            coloredBombPicker = new ColoredBombPicker(gameVariables);
            bombPlacer = new ColoredBombPlacer(gameBoard, coloredBombPicker);

            backgroundStrategy = new ParallelFillInstantStrategy(gameBoard, bgGemSpawner, gemMover, coroutineRunner, gameLogic);
            //gemsFillStrategy = new SequentialFillAnimatedStrategy(gameBoard, gemSpawner, gemMover, gameLogic);
            gemsFillStrategy = new CascadeRefillStrategy(gameBoard, gemSpawner, gemMover, gameVariables);
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

        public IEnumerator RefillBoardCor()
        {
            return gemsFillStrategy.ReFill(gemsRoot);
        }

        public void PlaceBomb(BombSpawnDecision spawnDecision)
        {
            bombPlacer.PlaceBombAt(spawnDecision.cell, spawnDecision.gemColor, spawnDecision.gemType, gemsRoot);
        }

    }

}