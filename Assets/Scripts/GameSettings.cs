using UnityEngine;
using static Chess.Game.GameManager;

[CreateAssetMenu(fileName ="GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    public PlayerType whitePlayerType;
    public PlayerType blackPlayerType;
    public bool WhiteIsBottom;
}
