using UnityEngine;

namespace FillBoardLogic.Data
{

    [CreateAssetMenu(fileName = "StrategyAnimationData", menuName = "Data/StrategyAnimationData")]
    class StrategyAnimationsData : ScriptableObject
    {
        public float duration;
        public AnimationCurve curve;
    }
}
