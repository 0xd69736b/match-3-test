using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnums : MonoBehaviour
{
    public enum GemType { regular, bomb };
    public enum GemColor { blue, green, red, yellow, purple, none};
    public enum GameState { wait, move }
}
