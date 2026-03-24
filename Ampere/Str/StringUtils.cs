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
                        spaceSplit[i] = new string(Shuffler.Shuffle(spaceSplit[i].ToCharArray()));
                    }
                }

                return string.Join(" ", spaceSplit);
            }
        }

        return new string(Shuffler.Shuffle(str.ToCharArray()));
    }

    /// <summary>
    /// Returns true if the length of the string is zero or one.
    /// </summary>
    /// <param name="str">The string to be used</param>
    /// <returns>True if the length of the string is zero or one</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsZeroOrOne(this string str) => str.Length is 0 or 1;
} //StringUtils
