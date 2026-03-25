using UnityEngine;

public static class ColorService
{
    public static readonly Color Even = Hex("#4BFF99");
    public static readonly Color Odd = Hex("#FD314E");
    public static readonly Color Clubs = Hex("#A9C338");
    public static readonly Color Cups = Hex("#FA654E");
    public static readonly Color Swords = Hex("#84E5E5");
    public static readonly Color Golds = Hex("#FDF61E");

    static Color Hex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var color))
            return color;

        Debug.LogError($"Invalid hex color: {hex}");
        return Color.magenta;
    }
}