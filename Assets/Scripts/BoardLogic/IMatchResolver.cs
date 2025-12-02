using System.Collections;
using UnityEngine;

namespace BoardLogic
{
    interface IMatchResolver
    {
        bool Scan(out MatchScanResult scanResult, Vector2Int? initiatorCell = null);
        IEnumerator ResolveStep(Vector2Int? initiatorCell = null);
        IEnumerator ResolveAll(Vector2Int? initiatorCell = null);
    }
}
