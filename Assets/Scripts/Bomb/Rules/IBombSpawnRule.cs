using BoardLogic;
using System.Collections.Generic;
using UnityEngine;

namespace Bomb.Rules
{
    public interface IBombSpawnRule
    { 
        List<BombSpawnDecision> Decide(IReadOnlyList<MatchCluster> clusters, Vector2Int? initiatorCell);
    }
}
