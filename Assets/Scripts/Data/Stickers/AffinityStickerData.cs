using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Affinity")]
    public class AffinityStickerData : StickerData
    {
        public Suit strongSuit;
        public float strongMultiplier = 2f;

        public Suit weakSuit;
        public int weakValue = 3;

        public override ISticker Create() =>
            new AffinitySticker(strongSuit, weakSuit, strongMultiplier, weakValue);

        public override string GetDescription()
        {
            var strongSuitColored = RichText.Colorize(
                strongSuit.ToString(),
                RichText.GetSuitColor(strongSuit)
            );

            var weakSuitColored = RichText.Colorize(
                weakSuit.ToString(),
                RichText.GetSuitColor(weakSuit)
            );

            return descriptionTemplate
                   .Replace("{strongMultiplier}", strongMultiplier.ToString())
                   .Replace("{weakValue}", weakValue.ToString())
                   .Replace("{strongSuit}", strongSuitColored)
                   .Replace("{weakSuit}", weakSuitColored);
        }
    }
}

public static class RichText
{
    public static string Colorize(string text, Color color) =>
        $"<color=#{color.ToHex()}>{text}</color>";

    public static Color GetSuitColor(Suit suit) =>
        suit switch
        {
            Suit.Clubs => ColorService.Clubs,
            Suit.Cups => ColorService.Cups,
            Suit.Swords => ColorService.Swords,
            Suit.Golds => ColorService.Golds,
            _ => Color.white
        };
}

public static class ColorExtensions
{
    public static string ToHex(this Color color) =>
        ColorUtility.ToHtmlStringRGB(color);
}