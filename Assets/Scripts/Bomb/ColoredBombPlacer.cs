using FillBoardLogic;
using Pooling;
using UnityEngine;

namespace Bomb
{
    class ColoredBombPlacer : IBombPlacer
    {
        private readonly GameBoard gameBoard;
        private readonly IColoredBombPicker bombPicker;

        public ColoredBombPlacer(GameBoard gameBoard, IColoredBombPicker bombPicker)
        {
            this.gameBoard = gameBoard;
            this.bombPicker = bombPicker;
        }

        public void PlaceBombAt(Vector2Int cell, GlobalEnums.GemColor color, GlobalEnums.GemType gemType, Transform parent)
        {
            GameObject bombPrefab = bombPicker.PickBomb(color);
            SC_Gem bombInstance = bombPrefab.Pool<SC_Gem>();
            bombInstance.SetParent(parent);
            bombInstance.SetPositionAndRotation(new Vector3(cell.x, cell.y), Quaternion.identity);
            gameBoard.SetGem(cell.x, cell.y, bombInstance);
            bombInstance.SetupGem(cell);
        }

    }
}
