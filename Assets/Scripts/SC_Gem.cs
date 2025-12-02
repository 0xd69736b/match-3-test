using System;
using UnityEngine;

public class SC_Gem : PoolObject, IEquatable<SC_Gem>
{
    [HideInInspector]
    public Vector2Int posIndex;

    public GlobalEnums.GemType type;
    public GlobalEnums.GemColor color;
    public bool isMatch = false;
    public GameObject destroyEffect;
    public int scoreValue = 10;
    public int blastSize = 1;

    public void SetupGem(Vector2Int _Position)
    {
        posIndex = _Position;
    }

    public bool Equals(SC_Gem other)
    {
        return color == other.color && type == other.type;
    }
}
