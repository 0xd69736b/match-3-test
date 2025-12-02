using System.Collections.Generic;
using UnityEngine;

namespace Bomb
{
    class BombActivationResult
    {
        public readonly List<Vector2Int> targets = new List<Vector2Int>(50);
        public readonly List<Vector2Int> bombCenters = new List<Vector2Int>(16);
    }
}
