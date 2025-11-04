using BoardLogic;
using FillBoardLogic.Spawning;
using System;
using System.Collections;
using UnityEngine;

namespace FillBoardLogic
{
    public class SpawnedGemArgs
    {
        public PoolObject gem;
        public int x;
        public int y;
    }

    public class BoardGemOrchestrator
    {
        private readonly IGemSpawner gemSpawner;
        private readonly GemMover gemMover;

        private SpawnedGemArgs gemArgs;
        private Action<SpawnedGemArgs> onCompleted;
        private bool moveFinished;
        private WaitUntil waiter;

        public BoardGemOrchestrator(IGemSpawner gemSpawner, GemMover gemMover)
        {
            this.gemSpawner = gemSpawner;
            this.gemMover = gemMover;
            waiter = new WaitUntil(IsMoveCompleted);
        }

        public IEnumerator SpawnInstant(int x, int y, Transform parent, Action<SpawnedGemArgs> onCompleted)
        {
            yield return gemSpawner.Spawn(x, y, parent, onCompleted);
        }

        public IEnumerator SpawnWithFall(int x, int y, Transform parent, float duration, AnimationCurve curve, Action<SpawnedGemArgs> onCompleted)
        {
            this.onCompleted = onCompleted;
            yield return gemSpawner.Spawn(x, y, parent, OnGemSpawned);
            moveFinished = false;
            gemMover.EnqueueMove(gemArgs.gem, new Vector2Int(x, y), duration, curve, OnMoveCompleted);
            yield return waiter;
        }

        private void OnGemSpawned(SpawnedGemArgs gemArgs)
        {
            this.gemArgs = gemArgs;
        }

        private void OnMoveCompleted(GemMover.MoveRequest moveRequest)
        {
            moveFinished = true;
            onCompleted?.Invoke(gemArgs);
        }

        private bool IsMoveCompleted() => moveFinished;

        public IEnumerator WaitUntilIdle() => gemMover.IdleWait;

    }
}
