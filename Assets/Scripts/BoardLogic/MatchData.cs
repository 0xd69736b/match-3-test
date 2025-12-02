using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoardLogic
{
    public class MatchLine: IEquatable<MatchLine>
    {
        public List<Vector2Int> CellsSorted;
        public Vector2Int TriggerCell;
        public GlobalEnums.LineOrientation Orientation;
        public GlobalEnums.GemType Type;
        public GlobalEnums.GemColor Color;

        public bool Equals(MatchLine other)
        {
            return Type == other.Type && Color == other.Color;
        }
    }

    public class MatchScanResult
    {
        public List<MatchLine> Triples;
        public List<MatchLine> FoursOrMore;
    }
}
