using UnityEngine;

namespace FillBoardLogic
{
    public class ColoredBombPicker : IColoredBombPicker
    {
        private readonly SC_GameVariables gameVariables;

        public ColoredBombPicker(SC_GameVariables gameVariables)
        {
            this.gameVariables = gameVariables;
        }

        public GameObject PickBomb(GlobalEnums.GemColor gemColor)
        {
            SC_Gem[] coloredBombs = gameVariables.coloredBombs;
            for(int i = 0; i < coloredBombs.Length; ++i)
            {
                if (coloredBombs[i].color == gemColor)
                    return coloredBombs[i].GO;
            }

            return gameVariables.bomb.GO;
        }
    }
}
