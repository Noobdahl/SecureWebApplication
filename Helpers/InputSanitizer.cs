using System.Text.RegularExpressions;

public static class InputSanitizer
{
    public static string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove SQL injection patterns
        input = Regex.Replace(input, @"(['"";]|--|\b(OR|AND|SELECT|INSERT|DELETE|UPDATE|DROP|EXEC|UNION|CREATE|ALTER|TRUNCATE|MERGE|CALL|DECLARE|CAST)\b)", string.Empty, RegexOptions.IgnoreCase); // Remove common SQL injection patterns

        // Remove potentially harmful HTML tags
        input = Regex.Replace(input, @"<.*?>", string.Empty); // Remove HTML tags

        // Allow only alphanumeric characters, whitespace, @, ., and -
        input = Regex.Replace(input, @"[^\w\s@.-]", string.Empty);

        return input.Trim();
    }
}