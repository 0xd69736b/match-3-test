using UnityEngine;

namespace BoardLogic
{
    class InputController
    {

        private readonly Camera mainCam;
        private readonly GameBoard gameBoard;
        private readonly GemSwapController gemSwapController;
        private readonly SC_GameLogic gameLogic;
        private readonly SC_GameVariables gameVariables;
        private Vector2 startScreenPos;
        private Vector2 endScreenPos;
        private bool mousePressed;
        private float swipeAngle = 0;
        private float minDist = 0.5f;

        public InputController(Camera mainCam, GameBoard gameBoard, GemSwapController gemSwapController, SC_GameLogic gameLogic, SC_GameVariables gameVariables)
        {
            this.mainCam = mainCam;
            this.gameBoard = gameBoard;
            this.gemSwapController = gemSwapController;
            this.gameLogic = gameLogic;
            this.gameVariables = gameVariables;
        }

        public void HandleInput()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if (gameLogic.CurrentState == GlobalEnums.GameState.move)
                {
                    startScreenPos = Input.mousePosition;
                    mousePressed = true;
                }
            }

            if (mousePressed && Input.GetMouseButtonUp(0))
            {
                mousePressed = false;
                if (gameLogic.CurrentState == GlobalEnums.GameState.move)
                {
                    endScreenPos = Input.mousePosition;
                    TryDoSwap();
                }
            }

        }

        private bool TryPickCell(Vector2 screenPos, out Vector2Int cell)
        {
            Ray ray = mainCam.ScreenPointToRay(screenPos);
            cell = Vector2Int.zero;
            Debug.Log(mainCam.ScreenToWorldPoint(screenPos));

            Debug.DrawRay(ray.origin, ray.direction * byte.MaxValue, Color.red, 1);

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, byte.MaxValue, gameVariables.gemLayer);
            if(hit.collider == null)
                return false;

            bool hasResult = gameBoard.TryGetGemCell(hit.collider.gameObject, out cell);
            if (!hasResult)
                return false;

            return gameBoard.InBounds(cell.x, cell.y);
        }

        private void TryDoSwap()
        {
            Debug.Log($"Start: {startScreenPos} end: {endScreenPos}");

            if (!TryPickCell(startScreenPos, out Vector2Int startCell))
                return;

            float dist = Vector3.Distance(startScreenPos, endScreenPos);
            if (dist < minDist)
                return;

            Vector2Int moveDir = GetMoveDir();
            Vector2Int endCell = startCell + moveDir;

            if (!gameBoard.InBounds(endCell) || startCell == endCell)
                return;

            gemSwapController.TrySwap(startCell, endCell, endCell);
        }

        private Vector2Int GetMoveDir()
        {
            CalculateAngle();

            if (swipeAngle < 45 && swipeAngle > -45)
            {
                return Vector2Int.right;
            }
            else if (swipeAngle > 45 && swipeAngle <= 135)
            {
                return Vector2Int.up;
            }
            else if (swipeAngle < -45 && swipeAngle >= -135)
            {
                return Vector2Int.down;
            }
            else if (swipeAngle > 135 || swipeAngle < -135)
            {
                return Vector2Int.left;
            }

            return Vector2Int.zero;
        }

        private void CalculateAngle()
        {
            swipeAngle = Mathf.Atan2(endScreenPos.y - startScreenPos.y, endScreenPos.x - startScreenPos.x);
            swipeAngle = swipeAngle * 180 / Mathf.PI;
        }

    }
}
