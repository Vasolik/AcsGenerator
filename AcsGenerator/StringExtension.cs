namespace Vipl.AcsGenerator;

public static class StringExtension
{
    public static string Intend(this string value, int count, bool first = false)
    {
        var indent = string.Join("", Enumerable.Repeat("    ", count));
        return (first ? indent: "") + value.Replace("\n", "\n" + indent);
    }
    public static string[] Tokenized(this string value, bool allWhite = false, string separators = "\n\r")
    {
        if (allWhite)
        {
            separators = "\t\n\r ";
        }
        return value.Split(separators.ToArray())
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();
    }

    public static string Join (this IEnumerable<string> values, int intend = 0,  bool first = false, string separator = "\n")
    {
        return string.Join(separator, values.Where(v => v is not null)).Intend(intend, first);
    }
        
    public static bool IsNullOrEmpty (this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static int ToInt(this string value)
    {
        return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }
        
    public static bool ToBool(this string value)
    {
        return !string.IsNullOrEmpty(value) && bool.Parse(value);
    }
       
}
    
public static class EnumerationExtension
{
    public static IEnumerable<T> MakeEnumerable<T>(this T value)
    {
        yield return value;
    }
    public static IEnumerable<T> MakeEnumerable<T>(this T value, IEnumerable<T> others)
    {
        yield return value;
        foreach (var other in others)
        {
            yield return other;
        }
    }
    public static T[] MakeArray<T>(this T value)
    {
        return value.MakeEnumerable().ToArray();
    }
    public static T[] MakeArray<T>(this T value, IEnumerable<T> others)
    {
        return value.MakeEnumerable(others).ToArray();
    }
       
}