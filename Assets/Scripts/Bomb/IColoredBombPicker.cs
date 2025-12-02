using UnityEngine;

namespace FillBoardLogic
{

    public interface IColoredBombPicker
    {
        GameObject PickBomb(GlobalEnums.GemColor gemColor);
    }

}