using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace BoardLogic
{
    public class GemSwapController 
    {
        private readonly GameBoard gameBoard;
        private readonly GemMover gemMover;
        private readonly MatchDetector matchDetector;
        private readonly CoroutineRunner runner;

        public event System.Action<Vector2Int?, MatchScanResult> OnSwapWithMatch;
        public event System.Action<Vector2Int, Vector2Int> OnSwapWithoutMatch;
        public event System.Action OnBeginSwap;

        private bool isMoveCompleted;
        private WaitUntil waiter;

        public GemSwapController(GameBoard gameBoard, GemMover gemMover, MatchDetector matchDetector, CoroutineRunner runner)
        {
            this.gameBoard = gameBoard;
            this.gemMover = gemMover;
            this.matchDetector = matchDetector;
            this.runner = runner;
            waiter = new WaitUntil(WaiterPredicate);
        }

        public void TrySwap(Vector2Int a, Vector2Int b, Vector2Int? initiator)
        {
            if (!AreNeighbours(a, b)) 
                return;
            if (!gameBoard.InBounds(a.x, a.y) || !gameBoard.InBounds(b.x, b.y)) 
                return;

            runner.RunCoroutine(SwapRoutine(a, b, initiator));
        }

        private IEnumerator SwapRoutine(Vector2Int a, Vector2Int b, Vector2Int? initiator)
        {
            SC_Gem goA = gameBoard.GetGem(a.x, a.y);
            SC_Gem goB = gameBoard.GetGem(b.x, b.y);

            if (goA == null || goB == null)
                yield break;

            OnBeginSwap?.Invoke();

            var toA = goB.posIndex;
            var toB = goA.posIndex;

            List<PoolObject> pair = new List<PoolObject>() { goA, goB };
            List<Vector2Int> endPos = new List<Vector2Int>() { toA, toB };

            isMoveCompleted = false;

            gemMover.EnqueueMove(pair, endPos, OnMoveCompleted);
            yield return waiter;

            gameBoard.SetGem(a.x, a.y, goB);
            goB.SetupGem(toB);
            gameBoard.SetGem(b.x, b.y, goA);
            goA.SetupGem(toA);

            bool hasMatches = matchDetector.FindAllMatches(out MatchScanResult matchScanResult, initiator);

            if (hasMatches)
            {
                OnSwapWithMatch?.Invoke(initiator, matchScanResult);
                yield break;
            }

            isMoveCompleted = false;
            gemMover.EnqueueMove(pair, new List<Vector2Int> { toB, toA }, OnMoveCompleted);
            yield return waiter;

            gameBoard.SetGem(a.x, a.y, goA);
            goA.SetupGem(toB);
            gameBoard.SetGem(b.x, b.y, goB);
            goB.SetupGem(toA);

            OnSwapWithoutMatch?.Invoke(a, b);
        }

        private static bool AreNeighbours(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;

        private void OnMoveCompleted(GemMover.MoveRequest moveRequest)
        {
            isMoveCompleted = true;
        }

        private bool WaiterPredicate() => isMoveCompleted;
    }
}
