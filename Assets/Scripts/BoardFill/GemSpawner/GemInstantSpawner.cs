using Pooling;
using System;
using System.Collections;
using UnityEngine;

namespace FillBoardLogic.Spawning
{

    public class GemInstantSpawner : IGemSpawner
    {
        private readonly IGemPicker gemPicker;
        private readonly SC_GameVariables gameVariables;

        public GemInstantSpawner(IGemPicker gemPicker, SC_GameVariables gameVariables)
        {
            this.gemPicker = gemPicker;
            this.gameVariables = gameVariables;
        }

        public IEnumerator Spawn(int x, int y, Transform parent, Action<SpawnedGemArgs> onCompleted)
        {
            GameObject prefab = gemPicker.PickGem(x, y);
            SC_Gem instance = prefab.Pool<SC_Gem>();
            instance.name = $"Gem - {x}, {y}";
            Vector3 pos = new Vector3(x, y + gameVariables.dropHeight, 0);
            instance.SetParent(parent);
            instance.SetPositionAndRotation(pos, Quaternion.identity);

            SpawnedGemArgs gemArgs = new SpawnedGemArgs()
            {
                gem = instance,
                x = x,
                y = y
            };

            onCompleted?.Invoke(gemArgs);

            yield return null;
        }

    }

}