using System.Collections.Generic;
using UnityEngine;

namespace BoardLogic
{

    public sealed class BoardIndexCache
    {
        private readonly Dictionary<int, Vector2Int> idToCell = new();
        private readonly Dictionary<Vector2Int, GameObject> cellToGo = new();
        private readonly Dictionary<int, GameObject> idToGo = new();

        public void OnSet(int x, int y, GameObject go)
        {
            var cell = new Vector2Int(x, y);

            if (cellToGo.TryGetValue(cell, out var oldGo))
            {
                if (oldGo)
                {
                    int oldId = oldGo.GetInstanceID();
                    idToCell.Remove(oldId);
                    idToGo.Remove(oldId);
                }
                cellToGo.Remove(cell);
            }

            if (!go) return;

            int id = go.GetInstanceID();
            if (idToCell.TryGetValue(id, out var prevCell))
            {
                if (cellToGo.TryGetValue(prevCell, out var prevGo) && prevGo == go)
                    cellToGo.Remove(prevCell);
            }

            idToCell[id] = cell;
            idToGo[id] = go;
            cellToGo[cell] = go;
        }

        public bool TryGetCell(GameObject go, out Vector2Int cell)
        {
            cell = default;
            if (!go) return false;
            int id = go.GetInstanceID();

            if (!idToGo.TryGetValue(id, out var alive) || !alive)
            {
                idToGo.Remove(id);
                idToCell.Remove(id);
                return false;
            }

            return idToCell.TryGetValue(id, out cell);
        }

        public void Unregister(GameObject go)
        {
            if (!go) return;
            int id = go.GetInstanceID();

            if (idToCell.TryGetValue(id, out var cell))
            {
                if (cellToGo.TryGetValue(cell, out var cur) && cur == go)
                    cellToGo.Remove(cell);
                idToCell.Remove(id);
                idToGo.Remove(id);
            }
            else
            {
                idToGo.Remove(id);
            }
        }

    }

}
