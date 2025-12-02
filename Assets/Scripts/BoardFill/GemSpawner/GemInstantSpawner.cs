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
        private readonly GameBoard gameBoard;

        public GemInstantSpawner(IGemPicker gemPicker, SC_GameVariables gameVariables, GameBoard gameBoard)
        {
            this.gemPicker = gemPicker;
            this.gameVariables = gameVariables;
            this.gameBoard = gameBoard;
        }

        public IEnumerator Spawn(int x, int y, Transform parent, Action<SpawnedGemArgs> onCompleted)
        {
            GameObject prefab = gemPicker.PickGem(x, y);
            SC_Gem instance = prefab.Pool<SC_Gem>();
            instance.name = $"Gem - {x}, {y}";
            Vector3 pos = new Vector3(x, gameBoard.Height - 1 + gameVariables.dropHeight, 0);
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
            instance.name = $"Gem - {x}, {y}";
            Vector3 pos = new Vector3(spawnPos.x, spawnPos.y, 0);
            instance.SetParent(parent);
            instance.SetPositionAndRotation(pos, Quaternion.identity);
            return instance;
        }
    }

}