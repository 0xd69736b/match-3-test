using UnityEngine;

namespace FillBoardLogic.Spawning
{

    public interface IGemPicker
    {
        GameObject PickGem(int x, int y);
    }

}