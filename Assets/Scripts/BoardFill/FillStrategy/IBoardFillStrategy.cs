using System.Collections;
using UnityEngine;

namespace FillBoardLogic
{

    public interface IBoardFillStrategy
    {
        void Init();
        IEnumerator Fill(Transform parent, bool addToGameBoard = true);
        IEnumerator ReFill(Transform parent);
    }

}