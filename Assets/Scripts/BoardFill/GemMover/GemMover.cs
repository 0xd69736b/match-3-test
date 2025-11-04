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
            public List<PoolObject> target;
            public List<Vector2Int> endPos;
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
                target = new List<PoolObject> { target },
                endPos = new List<Vector2Int> { endPoint },
                duration = duration,
                curve = curve,
                onCompleted = onCompleted
            };

            movesQueue.Enqueue(moveRequest);

            TryRun();
        }

        public void EnqueueMove(List<PoolObject> target, List<Vector2Int> endPoint, float duration, AnimationCurve curve, Action<MoveRequest> onCompleted = null)
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
                target = new List<PoolObject> { target },
                endPos = new List<Vector2Int> { endPoint },
                duration = defaultData.duration,
                curve = defaultData.curve,
                onCompleted = onCompleted
            };

            movesQueue.Enqueue(moveRequest);

            TryRun();
        }

        public void EnqueueMove(MoveRequest moveRequest)
        {
            movesQueue.Enqueue(moveRequest);
            TryRun();
        }

        public void EnqueueMove(List<PoolObject> target, List<Vector2Int> endPoint, Action<MoveRequest> onCompleted = null)
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

                List<PoolObject> targets = request.target;
                List<Vector2Int> endPoses = request.endPos;
                Vector3[] startPoses = new Vector3[targets.Count];
                for(int i = 0; i < targets.Count; ++i)
                {
                    startPoses[i] = targets[i].Tr.position;
                }

                float timePassed = 0;
                float duration = request.duration;
                AnimationCurve curve = request.curve;

                while (timePassed < duration)
                {
                    timePassed += Time.smoothDeltaTime;
                    float t = timePassed / duration;
                    float et = curve.Evaluate(t);
                    for(int i = 0; i < targets.Count; ++i)
                    {
                        Vector2Int endpos = endPoses[i];
                        Vector3 pos = Vector3.LerpUnclamped(startPoses[i], new Vector3(endpos.x, endpos.y), et);
                        targets[i].Tr.position = pos;
                    }
                    yield return null;
                }

                for (int i = 0; i < targets.Count; ++i)
                {
                    Vector2Int endpos = endPoses[i];
                    request.target[i].Tr.position = new Vector3(endpos.x, endpos.y);
                }
                request.onCompleted?.Invoke(request);
            }

            isRunning = false;
        }

        private bool IsIdle() => !isRunning && movesQueue.Count == 0;
    }

}