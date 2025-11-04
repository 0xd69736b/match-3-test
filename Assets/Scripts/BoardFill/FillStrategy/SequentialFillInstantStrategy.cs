using BoardLogic;
using FillBoardLogic.Spawning;
using System.Collections;
using UnityEngine;

namespace FillBoardLogic
{
    class SequentialFillInstantStrategy : SequentialFillStrategy
    {
        public SequentialFillInstantStrategy(GameBoard gameBoard, IGemSpawner gemSpawner, GemMover gemMover, SC_GameLogic gameLogic) : base(gameBoard, gemSpawner, gemMover, gameLogic)
        {
        }

        protected override IEnumerator CallSpawn(int x, int y, Transform parent)
        {
            return orchestrator.SpawnInstant(x, y, parent, OnGemSpawned);
        }
    }
}
