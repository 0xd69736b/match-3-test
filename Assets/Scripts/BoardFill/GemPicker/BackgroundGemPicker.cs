using UnityEngine;

namespace FillBoardLogic.Spawning
{
    class BackgroundGemPicker : IGemPicker
    {
        private readonly SC_GameVariables gameVariables;

        public BackgroundGemPicker(SC_GameVariables gameVariables)
        {
            this.gameVariables = gameVariables;
        }

        public GameObject PickGem(int x, int y)
        {
            return gameVariables.bgTilePrefabs;
        }
    }
}
