using BoardLogic;
using FillBoardLogic.Spawning;
using System.Collections;
using UnityEngine;

namespace FillBoardLogic
{
    class SequentialFillAnimatedStrategy : SequentialFillStrategy
    {
        public SequentialFillAnimatedStrategy(GameBoard gameBoard, IGemSpawner gemSpawner, GemMover gemMover, SC_GameLogic gameLogic) : base(gameBoard, gemSpawner, gemMover, gameLogic)
        {
        }

        protected override IEnumerator CallSpawn(int x, int y, Transform parent)
        {
            return orchestrator.SpawnWithFall(x, y, parent, duration, curve, OnGemSpawned);
        }
    }
}
