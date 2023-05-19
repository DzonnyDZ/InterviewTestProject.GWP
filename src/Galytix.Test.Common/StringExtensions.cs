namespace Galytix.Test.Common;

/// <summary>Contains extension methods for working with strings</summary>
public static class StringExtensions
{
    /// <summary>Returns a copy of given string with first character converted to lowercase using the casing rules of the invariant culture.</summary>
    /// <param name="str">The string to convert first character of</param>
    /// <returns><paramref name="str"/> with first character converted to lower case</returns>
    public static string FirstLowerInvariant(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return str[0].ToString().ToLowerInvariant() + str[1..];
    }

    /// <summary>Returns a copy of given string with first character converted to lowercase using the casing rules of the invariant culture.</summary>
    /// <param name="str">The string to convert first character of</param>
    /// <returns><paramref name="str"/> with first character converted to lower case</returns>
    public static string FirstUpperInvariant(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return str[0].ToString().ToUpperInvariant() + str[1..];
    }
}
