
namespace Utils;
public enum TextAlignment
{
    LEFT,
    CENTER,
    RIGHT
}

public static class StringExtensions
{
    public static string ToFixedLength(this string text, int length, TextAlignment alignment = TextAlignment.CENTER)
    {
        var lengthDifference = length - text.Length;

        if(lengthDifference <= 0)
            return text[..length];

        var padLeftSize = 0;
        switch(alignment)
        {
            case TextAlignment.CENTER:
                var halfDifference = (double)lengthDifference / 2;
                padLeftSize = Convert.ToInt32(Math.Floor(halfDifference));
                break;
            case TextAlignment.LEFT:
                break;
            case TextAlignment.RIGHT:
                padLeftSize = lengthDifference;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(alignment), alignment,
                                                      "Invalid text alignment option");
        }

        return $"{new string(' ', padLeftSize)}{text}".PadRight(length);
    }
}
