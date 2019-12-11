using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// static class class for game constants
/// </summary>
public static class GameData
{
    public static readonly byte COLUMNS_COUNT = 6;
    public static readonly byte ROWS_COUNT = 6;
    
    public static readonly float BEGIN_DOT_POS_Y = 10f;
    public static readonly float ANIMATION_DELAY = 0.075f;

    public static readonly uint MIN_OBJECT_COUNT_POOL = 5;

    public static readonly float DOT_SPRITE_SIZE = 32f;
    public static readonly float DOT_SPACING = 0.5f;

    public static readonly float DOT_ANIMATION_TIME = 0.6f;

    public static string savePath;
    

    public static readonly Dictionary<DotColorType, Color> DotColors = new Dictionary<DotColorType, Color>()
    {
        {DotColorType.Red, new Color32(192, 57, 43, 255)},
        {DotColorType.Green, new Color32(39, 174, 96, 255)},
        {DotColorType.Blue, new Color32(41, 128, 185, 255)},
        {DotColorType.Cyan, new Color32(211, 84, 0, 255)},
        {DotColorType.Magenta, new Color32(142, 68, 173, 255)},
        {DotColorType.Yellow, new Color32(243, 156, 18, 255)},
        {DotColorType.Clear, Color.clear}, 
    };
}
