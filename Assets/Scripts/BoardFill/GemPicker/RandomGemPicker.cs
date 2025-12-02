using UnityEngine;

namespace FillBoardLogic.Spawning
{

    public class RandomGemPicker : IGemPicker
    {
        private readonly SC_GameVariables gameVariables;
        private readonly GameBoard gameBoard;

        public RandomGemPicker(SC_GameVariables gameVariables, GameBoard gameBoard)
        {
            this.gameVariables = gameVariables;
            this.gameBoard = gameBoard;
        }

        public GameObject PickGem(int x, int y)
        {
            int randomValue = Random.Range(0, 101);
            if (randomValue < gameVariables.bombChance)
                return gameVariables.bomb.GO;

            const int maxAttempts = 50;

            SC_Gem[] gems = gameVariables.gems;

            for(int i = 0; i < maxAttempts; ++i)
            {
                int gemIdx = Random.Range(0, gems.Length);
                SC_Gem gem = gems[gemIdx];
                if (!WouldMatch(x, y, gem))
                {
                    return gem.GO;
                }
            }
            int fallbackGemIdx = Random.Range(0, gems.Length);
            return gems[fallbackGemIdx].GO;
        }


        private bool WouldMatch(int x, int y, SC_Gem gem)
        {
            return gameBoard.MatchesAt(new Vector2Int(x, y), gem);
        }
    }

}