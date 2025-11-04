using FillBoardLogic.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace BoardLogic
{

    public class GemMover
    {
        public class MoveRequest
        {
            public PoolObject target;
            public Vector2Int endPos;
            public float duration;
            public AnimationCurve curve;
            public Action<MoveRequest> onCompleted;
        }


        private readonly Queue<MoveRequest> movesQueue;
        private readonly CoroutineRunner coroutineRunner;
        private bool isRunning;
        public readonly WaitUntil IdleWait;
        private StrategyAnimationsData defaultData;

        public GemMover(CoroutineRunner coroutineRunner)
        {
            this.coroutineRunner = coroutineRunner;
            movesQueue = new Queue<MoveRequest>(25);
            IdleWait = new WaitUntil(IsIdle);
        }

        internal void SetDefaultAnimationData(StrategyAnimationsData data)
        {
            defaultData = data;
        }

        public void EnqueueMove(PoolObject target, Vector2Int endPoint, float duration, AnimationCurve curve, Action<MoveRequest> onCompleted = null)
        {
            MoveRequest moveRequest = new MoveRequest()
            {
                target = target,
                endPos = endPoint,
                duration = duration,
                curve = curve,
                onCompleted = onCompleted
            };

            movesQueue.Enqueue(moveRequest);

            TryRun();
        }

        public void EnqueueMove(PoolObject target, Vector2Int endPoint, Action<MoveRequest> onCompleted = null)
        {
            MoveRequest moveRequest = new MoveRequest()
            {
                target = target,
                endPos = endPoint,
                duration = defaultData.duration,
                curve = defaultData.curve,
                onCompleted = onCompleted
            };

            movesQueue.Enqueue(moveRequest);

            TryRun();
        }

        private void TryRun()
        {
            if (isRunning)
                return;

            coroutineRunner.RunCoroutine(RunQueue());
        }

        private IEnumerator RunQueue()
        {
            isRunning = true;

            while(movesQueue.Count > 0)
            {
                MoveRequest request = movesQueue.Dequeue();

                Vector3 startPos = request.target.Tr.position;
                Vector3 endPos = new Vector3(request.endPos.x, request.endPos.y);
                float timePassed = 0;
                float duration = request.duration;
                AnimationCurve curve = request.curve;

                while (timePassed < duration)
                {
                    timePassed += Time.smoothDeltaTime;
                    float t = timePassed / duration;
                    Vector3 pos = Vector3.LerpUnclamped(startPos, endPos, curve.Evaluate(t));
                    request.target.Tr.position = pos;
                    yield return null;
                }

                request.target.Tr.position = endPos;
                request.onCompleted?.Invoke(request);
            }

            isRunning = false;
        }

        private bool IsIdle() => !isRunning && movesQueue.Count == 0;
    }

}