using UnityEngine;

namespace Bomb
{
    interface IBombPlacer
    {
        void PlaceBombAt(Vector2Int cell, GlobalEnums.GemColor color, GlobalEnums.GemType gemType, Transform parent);
    }
}
