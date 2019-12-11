using System;
using UnityEngine;

/// <summary>
/// Class for game state
/// stores some parameters, like a scores, currentLevel, mb the grid state
/// saves to memory by SaveManager
/// </summary>
[Serializable]
public class GameSession
{
    [HideInInspector]
    public int scores;
}
