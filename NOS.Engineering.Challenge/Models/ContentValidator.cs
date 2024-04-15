using System.Text.RegularExpressions;

namespace NOS.Engineering.Challenge.Models;

public class ContentValidator
{
    private static readonly Regex UrlRegex = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool IsValidImageUrl(string imageUrl)
    {
        return !string.IsNullOrWhiteSpace(imageUrl) && UrlRegex.IsMatch(imageUrl);
    }
}