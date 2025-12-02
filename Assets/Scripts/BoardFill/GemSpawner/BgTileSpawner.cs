using FillBoardLogic;
using FillBoardLogic.Spawning;
using Pooling;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BoardFill.GemSpawner
{
    class BgTileSpawner : IGemSpawner
    {

        private readonly IGemPicker gemPicker;

        public BgTileSpawner(IGemPicker gemPicker)
        {
            this.gemPicker = gemPicker;
        }

        public IEnumerator Spawn(int x, int y, Transform parent, Action<SpawnedGemArgs> onCompleted)
        {
            GameObject prefab = gemPicker.PickGem(x, y);
            PoolObject instance = prefab.Pool<PoolObject>();
            instance.name = $"BG Tile - {x}, {y}";
            Vector3 pos = new Vector3(x, y, 0);
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

        public PoolObject Spawn(int x, int y, Vector2Int spawnPos, Transform parent)
        {
            GameObject prefab = gemPicker.PickGem(x, y);
            SC_Gem instance = prefab.Pool<SC_Gem>();
            instance.name = $"BG Tile - {x}, {y}";
            Vector3 pos = new Vector3(spawnPos.x, spawnPos.y, 0);
            instance.SetParent(parent);
            instance.SetPositionAndRotation(pos, Quaternion.identity);
            return instance;
        }
    }
}
