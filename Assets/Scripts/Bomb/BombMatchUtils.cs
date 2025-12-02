using UnityEngine;

namespace Bomb
{
    static class BombMatchUtils
    {
        public static bool IsBombInLineMatch(GameBoard gameBoard, Vector2Int bombCell)
        {
            return TryAxis(gameBoard, bombCell, Vector2Int.right) || TryAxis(gameBoard, bombCell, Vector2Int.up);
        }

        private static bool TryAxis(GameBoard gameBoard, Vector2Int center, Vector2Int axis)
        {
            if (!TryFirstNormalType(gameBoard, center, axis, out GlobalEnums.GemColor color) &&
               !TryFirstNormalType(gameBoard, center, -axis, out color))
                return false;

            int cnt = 0;
            cnt += CountSameNormal(gameBoard, center, axis, color);
            cnt += CountSameNormal(gameBoard, center, -axis, color);

            return cnt >= 2;
        }

        private static bool TryFirstNormalType(GameBoard gameBoard, Vector2Int start, Vector2Int step, out GlobalEnums.GemColor color)
        {
            color = GlobalEnums.GemColor.none;
            Vector2Int nextCell = start + step;
            if (!gameBoard.InBounds(nextCell))
                return false;

            SC_Gem gem = gameBoard.GetGem(nextCell.x, nextCell.y);
            if (gem == null || gem.type != GlobalEnums.GemType.regular)
                return false;

            color = gem.color;
            return true;
        }
        
        private static int CountSameNormal(GameBoard gameBoard, Vector2Int start, Vector2Int step, GlobalEnums.GemColor color)
        {
            int k = 0;
            Vector2Int nextCell = start + step;
            while(gameBoard.InBounds(nextCell))
            {
                SC_Gem gem = gameBoard.GetGem(nextCell.x, nextCell.y);
                if (gem == null || gem.type != GlobalEnums.GemType.regular || gem.color != color)
                    break;

                ++k;
                nextCell += step;
            }
            return k;
        }

    }
}
