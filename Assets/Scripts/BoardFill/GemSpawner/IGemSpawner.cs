using Pooling;
using System;
using System.Collections;
using UnityEngine;

namespace FillBoardLogic.Spawning
{
    public interface IGemSpawner
    {
        IEnumerator Spawn(int x, int y, Transform parent, Action<SpawnedGemArgs> onCompleted);
    }

}