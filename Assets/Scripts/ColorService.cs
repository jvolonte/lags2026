using UnityEngine;

public static class ColorService
{
    public static readonly Color Even = Hex("#4BFF99");
    public static readonly Color Odd = Hex("#FD314E");
    public static readonly Color Clubs = Hex("#A9C338");
    public static readonly Color Cups = Hex("#FA654E");
    public static readonly Color Swords = Hex("#84E5E5");
    public static readonly Color Golds = Hex("#FDF61E");

    public static readonly Color CharBlueGrey = Hex("#7C889E");
    public static readonly Color CharGold = Hex("#BE9834");
    public static readonly Color CharRed = Hex("#823122");
    public static readonly Color CharDark = Hex("#343647");
    public static readonly Color CharOlive = Hex("#818867");
    
    static Color Hex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var color))
            return color;

        Debug.LogError($"Invalid hex color: {hex}");
        return Color.magenta;
    }
}