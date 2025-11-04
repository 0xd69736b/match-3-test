using BoardLogic;
using FillBoardLogic.Spawning;
using UnityEngine;
using Utility;

namespace FillBoardLogic
{
    class ParallelFillInstantStrategy : ParallelFillStrategy
    {
        public ParallelFillInstantStrategy(GameBoard gameBoard, IGemSpawner gemSpawner, GemMover gemMover, CoroutineRunner coroutineRunner, SC_GameLogic gameLogic) : base(gameBoard, gemSpawner, gemMover, coroutineRunner, gameLogic)
        {
        }

        protected override void CallSpawn(int x, int y, Transform parent)
        {
            coroutineRunner.RunCoroutine(orchestrator.SpawnInstant(x, y, parent, OnGemSpawned));
        }
    }
}
