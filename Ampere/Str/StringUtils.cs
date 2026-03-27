#nullable enable
using Ampere.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Ampere.Str;

/// <summary>
/// A static utility class for string extension methods.
/// </summary>
public static class StringUtils
{
    private const char CharSpace = (char)32;

    /// <summary>
    /// Creates a string from the first character of the string to the nth whitespace that is specified.
    /// </summary>
    /// <param name="str">The string to be chomped</param>
    /// <param name="spaces">The amount of white space to chomp after</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>The string retaining the chomped word</returns>
    public static string ChompAfter(this string str, int spaces)
    {
        ArgumentNullException.ThrowIfNull(str);
        if (str.Length == 0)
        {
            return str;
        }

        if (spaces == 0) return str;
        var matchingNumSpaces = 0;
        for (var i = 0; i < str.Length; i++)
        {
            if (!char.IsWhiteSpace(str[i])) continue;
            matchingNumSpaces++;

            if (spaces == matchingNumSpaces)
            {
                return str[..i];
            }
        }

        return str;
    }

    /// <summary>
    /// Checks whether a string contains duplicate characters.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>True if there are duplicate characters. False, otherwise</returns>
    public static bool ContainsDuplicateChars(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (IsZeroOrOne(str))
        {
            return false;
        }

        var set = new HashSet<char>();
        return str.All(t => set.Add(t));
    }

    /// <summary>
    /// Checks whether a string contains duplicate inner strings.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <param name="arg">The inner string to search for duplicates</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="arg"/> is null</exception>
    /// <returns>True if there are duplicate inner strings. False, otherwise</returns>
    public static bool ContainsDuplicateStrings(this string str, string arg)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (IsZeroOrOne(str))
        {
            return false;
        }

        var regex = new Regex(Regex.Escape(arg));
        var rem = regex.Replace(str, string.Empty, 1);
        var secondRem = rem.Replace(arg, string.Empty);
        return rem != secondRem;
    }

    /// <summary>
    /// Counts the number of words in a string.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>The number of words in the string</returns>
    public static int CountWords(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);
        return str.Length switch
        {
            0 => 0,
            1 => 1,
            _ => str.Split([CharSpace, '\r', '\n'], options: StringSplitOptions.RemoveEmptyEntries)
                .Length
        };
    }

    /// <summary>
    /// Checks if a given string is a palindrome.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <param name="ignoreCase">Whether case should be ignored</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>True if the string is a palindrome</returns>
    public static bool IsPalindrome(this string str, bool ignoreCase = false)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (IsZeroOrOne(str))
        {
            return true;
        }

        if (ignoreCase)
        {
            str = str.ToUpperInvariant();
        }

        for (var i = 0; i < str.Length; i++)
        {
            var j = str.Length - 1 - i;
            if (str[i] != str[j])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if each character in a string is lexicographically smaller than the previous character.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <param name="ignoreCase">Whether case should be ignored</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>True if the string strictly increases</returns>
    public static bool IsStrictlyDecreasing(this string str, bool ignoreCase = false)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (IsZeroOrOne(str))
        {
            return false;
        }

        if (ignoreCase)
        {
            str = str.ToUpperInvariant();
        }

        for (var i = 0; i < str.Length - 1; i++)
        {
            if (str[i] < str[i + 1]) return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if each character in a string is lexicographically greater than the previous character.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <param name="ignoreCase">Whether case should be ignored</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>True if the string strictly increases</returns>
    public static bool IsStrictlyIncreasing(this string str, bool ignoreCase = false)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (IsZeroOrOne(str))
        {
            return false;
        }

        if (ignoreCase)
        {
            str = str.ToUpperInvariant();
        }

        for (var i = 0; i < str.Length - 1; i++)
        {
            if (str[i] > str[i + 1]) return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a given string is a valid date used by System.DateTime
    /// </summary>
    /// <param name="date">The string to be used</param>
    /// <param name="formattingRegex">The date format regex</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="date"/> is null</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="formattingRegex"/> is null</exception>
    /// <returns>True if the string is a valid date recognized by System.DateTime</returns>
    public static bool IsSystemDateTime(this string date, string formattingRegex)
    {
        ArgumentNullException.ThrowIfNull(date);
        ArgumentNullException.ThrowIfNull(formattingRegex);

        if (date.Length == 0 || formattingRegex.Length == 0)
        {
            return false;
        }

        return DateTime.TryParseExact(date, formattingRegex, CultureInfo.InvariantCulture, DateTimeStyles.None,
            out _);
    }

    /// <summary>
    /// Checks if a given string is a valid URI. This checks both HTTP and HTTPS URLs.
    /// </summary>
    /// <param name="uri">The string to be used</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="uri"/> is null</exception>
    /// <returns>True if the URI is valid</returns>
    public static bool IsValidUri(string uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        return Uri.TryCreate(uri, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Removes all instances of any number of characters from a specified string.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <param name="ignoreCase">Whether case should be ignored</param>
    /// <param name="args">The characters which will be removed</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is null</exception>
    /// <returns>The string with all characters in args removed</returns>
    public static string RemoveAll(this string str, bool ignoreCase = false, params IEnumerable<char>[] args)
    {
        ArgumentNullException.ThrowIfNull(str);
        ArgumentNullException.ThrowIfNull(args);
        if (args.Length == 0 || str.Length == 0)
        {
            return str;
        }

        if (ignoreCase)
        {
            str = str.ToUpperInvariant();
        }

        var sb = new StringBuilder(str);
        foreach (var t in args)
        {
            sb.Replace(t.ToString() ?? string.Empty, string.Empty);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Removes all instances of any number of strings from a specified string.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <param name="ignoreCase">Whether case should be ignored</param>
    /// <param name="args">The characters which will be removed</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is null</exception>
    /// <returns>The string with all characters in args removed</returns>
    public static string RemoveAll(this string str, bool ignoreCase = false, params IEnumerable<string>[] args)
    {
        ArgumentNullException.ThrowIfNull(str);
        ArgumentNullException.ThrowIfNull(args);
        if (args.Length == 0 || str.Length == 0)
        {
            return str;
        }

        if (ignoreCase)
        {
            str = str.ToUpperInvariant();
        }

        var sb = new StringBuilder(str);

        for (var i = 0; i < str.Length; i++)
        {
            int cnt = args[i].Count();
            var iStr = args[i].ToString();
            if (cnt == 1)
            {
                sb.Replace(iStr ?? string.Empty, string.Empty);
            }
            else
            {
                int idxOfWord = sb.ToString().IndexOf(iStr ?? string.Empty, StringComparison.InvariantCulture);
                sb.Remove(idxOfWord, cnt);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Replaces a character at a specific index in a string, only once.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <param name="c">The character to replace</param>
    /// <param name="index">The index to replace <paramref name="c"/></param>
    public static string ReplaceAt(this string str, int index, char c)
    {
        ArgumentNullException.ThrowIfNull(str);
        return new StringBuilder(str) { [index] = c }.ToString();
    }

    /// <summary>
    /// Reverses a string from left to right order while maintaining case sensitivity.
    /// </summary>
    /// <param name="str">The string to be reversed</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>The reversed string</returns>
    public static string Reverse(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (str.IsZeroOrOne())
        {
            return str;
        }

        var c = str.ToCharArray();
        Array.Reverse(c);
        return new string(value: c);
    }

    /// <summary>
    /// Shuffle's characters in a string. The methodology used to generate random
    /// indices used for shuffling is cryptographically strong. Due to this nature,
    /// there is no guarantee that the return string will be entirely different
    /// from the original.
    /// </summary>
    /// <param name="str">The string to be shuffled</param>
    /// <param name="preserveSpaces">Determines whether to shuffle spaces or not</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="str"/> is null</exception>
    /// <returns>The shuffled string</returns>
    public static string Shuffle(this string str, bool preserveSpaces = false)
    {
        ArgumentNullException.ThrowIfNull(str);
        if (str.IsZeroOrOne())
        {
            return str;
        }

        if (preserveSpaces)
        {
            if (str.Contains(CharSpace))
            {
                var spaceSplit = str.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < spaceSplit.Length; i++)
                {
                    if (!spaceSplit[i].IsZeroOrOne())
                    {
                        spaceSplit[i] = new string(Shuffler.ShuffleCopy((ReadOnlySpan<char>)spaceSplit[i].ToCharArray()));
                    }
                }

                return string.Join(" ", spaceSplit);
            }
        }

        return new string(Shuffler.ShuffleCopy((ReadOnlySpan<char>)str.ToCharArray()));
    }

    /// <summary>
    /// Truncates a string to a specified maximum length, appending an optional suffix if truncation occurs.
    /// </summary>
    /// <param name="str">The string to truncate</param>
    /// <param name="maxLength">The maximum length of the returned string (including suffix)</param>
    /// <param name="suffix">The suffix to append when truncation occurs (default: "...")</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxLength"/> is negative</exception>
    /// <returns>The original string if within bounds, or a truncated string with suffix</returns>
    public static string Truncate(this string str, int maxLength, string suffix = "...")
    {
        ArgumentNullException.ThrowIfNull(str);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);
        suffix ??= string.Empty;

        if (str.Length <= maxLength)
            return str;

        if (maxLength <= suffix.Length)
            return suffix[..maxLength];

        return string.Concat(str.AsSpan(0, maxLength - suffix.Length), suffix);
    }

    /// <summary>
    /// Determines whether the string contains any of the specified values.
    /// </summary>
    /// <param name="str">The string to search within</param>
    /// <param name="comparisonType">The comparison rules to use</param>
    /// <param name="values">The values to search for</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> or <paramref name="values"/> is null</exception>
    /// <returns>True if the string contains at least one of the specified values</returns>
    public static bool ContainsAny(this string str, StringComparison comparisonType, params string[] values)
    {
        ArgumentNullException.ThrowIfNull(str);
        ArgumentNullException.ThrowIfNull(values);

        foreach (var value in values)
        {
            if (str.Contains(value, comparisonType))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the string contains all of the specified values.
    /// </summary>
    /// <param name="str">The string to search within</param>
    /// <param name="comparisonType">The comparison rules to use</param>
    /// <param name="values">The values to search for</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> or <paramref name="values"/> is null</exception>
    /// <returns>True if the string contains all of the specified values</returns>
    public static bool ContainsAll(this string str, StringComparison comparisonType, params string[] values)
    {
        ArgumentNullException.ThrowIfNull(str);
        ArgumentNullException.ThrowIfNull(values);

        foreach (var value in values)
        {
            if (!str.Contains(value, comparisonType))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Repeats the string a specified number of times.
    /// </summary>
    /// <param name="str">The string to repeat</param>
    /// <param name="count">The number of times to repeat the string</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is negative</exception>
    /// <returns>A new string consisting of the original string repeated the specified number of times</returns>
    public static string Repeat(this string str, int count)
    {
        ArgumentNullException.ThrowIfNull(str);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        return count switch
        {
            0 => string.Empty,
            1 => str,
            _ => string.Create(str.Length * count, (str, count), static (span, state) =>
            {
                var (s, c) = state;
                for (var i = 0; i < c; i++)
                    s.AsSpan().CopyTo(span[(i * s.Length)..]);
            })
        };
    }

    /// <summary>
    /// Performs a case-insensitive ordinal equality comparison between two strings.
    /// </summary>
    /// <param name="str">The first string</param>
    /// <param name="other">The second string to compare</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>True if the strings are equal ignoring case</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsIgnoreCase(this string str, string other)
    {
        ArgumentNullException.ThrowIfNull(str);
        return str.Equals(other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a substring safely without throwing exceptions when the indices are out of range.
    /// If startIndex is beyond the string length, an empty string is returned.
    /// If startIndex + length exceeds the string length, the available characters are returned.
    /// </summary>
    /// <param name="str">The string to extract from</param>
    /// <param name="startIndex">The zero-based starting index</param>
    /// <param name="length">The maximum number of characters to extract</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>The substring, or an empty string if out of range</returns>
    public static string SafeSubstring(this string str, int startIndex, int length)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (startIndex < 0) startIndex = 0;
        if (length < 0) length = 0;

        if (startIndex >= str.Length)
            return string.Empty;

        if (startIndex + length > str.Length)
            length = str.Length - startIndex;

        return str.Substring(startIndex, length);
    }

    /// <summary>
    /// Removes all whitespace characters from the string.
    /// </summary>
    /// <param name="str">The string to process</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>The string with all whitespace characters removed</returns>
    public static string RemoveWhitespace(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (str.Length == 0)
            return str;

        return string.Create(str.Length, str, static (span, s) =>
        {
            var idx = 0;
            foreach (var c in s)
            {
                if (!char.IsWhiteSpace(c))
                    span[idx++] = c;
            }

            span[idx..].Fill('\0');
        }).TrimEnd('\0');
    }

    /// <summary>
    /// Converts a PascalCase or camelCase string to snake_case.
    /// </summary>
    /// <param name="str">The string to convert</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>The snake_case representation of the string</returns>
    public static string ToSnakeCase(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (str.Length <= 1)
            return str.ToLowerInvariant();

        var sb = new StringBuilder(str.Length + 4);
        sb.Append(char.ToLowerInvariant(str[0]));

        for (var i = 1; i < str.Length; i++)
        {
            var c = str[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts a PascalCase or camelCase string to kebab-case.
    /// </summary>
    /// <param name="str">The string to convert</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is null</exception>
    /// <returns>The kebab-case representation of the string</returns>
    public static string ToKebabCase(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (str.Length <= 1)
            return str.ToLowerInvariant();

        var sb = new StringBuilder(str.Length + 4);
        sb.Append(char.ToLowerInvariant(str[0]));

        for (var i = 1; i < str.Length; i++)
        {
            var c = str[i];
            if (char.IsUpper(c))
            {
                sb.Append('-');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns true if the length of the string is zero or one.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <returns>True if the length of the string is zero or one</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsZeroOrOne(this string str) => str.Length is 0 or 1;
} //StringUtils
