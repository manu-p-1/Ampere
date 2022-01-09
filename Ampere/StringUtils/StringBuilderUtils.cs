#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Ampere.Base.Attributes;

namespace Ampere.StringUtils
{
    /// <summary>
    /// A static utility class for StringBuilder extension methods.
    /// </summary>
    public static class StringBuilderUtils
    {
        /// <summary>
        /// A StringBuilder extension to append to the StringBuilder if and only if a condition is met.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="str">The string to append</param>
        /// <param name="condition">The condition to meet in order for the append to occur</param>
        /// <returns>The StringBuilder instance</returns>
        public static StringBuilder AppendIf(this StringBuilder sb, string str, bool condition)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));

            if (condition)
                sb.Append(str);

            return sb;
        }

        /// <summary>
        /// Appends the contents of an enumerable of generic objects provided a delegate to identify the string property.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="enumerable">The enumerable to append from</param>
        /// <param name="func">The function specifying what item should be appended</param>
        /// <returns>The StringBuilder instance</returns>
        public static StringBuilder AppendFromEnumerable<T>(this StringBuilder sb, IEnumerable<T> enumerable,
            Func<T, string> func)
            => EnumerableAction(sb, enumerable, s => sb.Append(func(s)));


        /// <summary>
        /// Appends the contents of a string enumerable. 
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="enumerable">The enumerable to append from</param>
        /// <returns>The StringBuilder instance</returns>
        public static StringBuilder AppendFromEnumerable(this StringBuilder sb, IEnumerable<string> enumerable)
            => EnumerableAction(sb, enumerable, s => sb.Append(s));

        /// <summary>
        /// Appends the contents of an enumerable of generic objects provided a delegate to identify the string property. This function also
        /// adds the appropriate line terminator at the end of the StringBuilder instance.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="enumerable">The enumerable to append from</param>
        /// <param name="func">The function specifying what item should be appended</param>
        /// <returns>The StringBuilder instance</returns>
        public static StringBuilder AppendLineFromEnumerable<T>(this StringBuilder sb, IEnumerable<T> enumerable,
            Func<T, string> func)
            => EnumerableAction(sb, enumerable, s => sb.AppendLine(func(s)));


        /// <summary>
        /// Appends the contents of a string enumerable. This function also
        /// adds the appropriate line terminator at the end of the StringBuilder instance.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="enumerable">The enumerable to append from</param>
        /// <returns>The StringBuilder instance</returns>
        public static StringBuilder AppendLineFromEnumerable(this StringBuilder sb, IEnumerable<string> enumerable)
            => EnumerableAction(sb, enumerable, s => sb.AppendLine(s));

        /// <summary>
        /// A helper method to carry out an enumerable action. This method really serves no function other than making the other
        /// methods look more elegant.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="enumerable">The enumerable to append from</param>
        /// <param name="action">The specific StringBuilder action to carry out for the StringBuilder</param>
        /// <returns>The StringBuilder instance</returns>
        private static StringBuilder EnumerableAction<T>(StringBuilder sb, IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var e in enumerable)
                action(e);
            return sb;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find as a character array</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <param name="count"></param>
        /// <param name="comparisonType"></param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta("Evaluating Performance versus StringBuilder.ToString().IndexOf()")]
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, int count,
            StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (startIndex < 0 || startIndex > sb.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0 || startIndex > sb.Length - count)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (value.Length == 0 && startIndex == 0)
            {
                return 0;
            }

            if (value.Length > sb.Length)
            {
                return -1;
            }


            int index;
            int length = value.Length;

            for (int i = startIndex; i < count + startIndex; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while (index < length &&
                           string.Equals(sb[i + index].ToString(), value[index].ToString(), comparisonType))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <param name="comparisonType">The <see cref="StringComparison"/> instance to specify culture and case rules</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, StringComparison comparisonType)
            => IndexOf(sb, value, startIndex, sb.Length - startIndex, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The character to find</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <param name="comparisonType">The <see cref="StringComparison"/> instance to specify culture and case rules</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, char value, int startIndex, StringComparison comparisonType)
            => IndexOf(sb, value.ToString(), startIndex, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <param name="comparisonType">The <see cref="StringComparison"/> instance to specify culture and case rules</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, string value, StringComparison comparisonType)
            => IndexOf(sb, value, 0, sb.Length, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, int count) =>
            IndexOf(sb, value, startIndex, count, StringComparison.CurrentCulture);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The character to find</param>
        /// <param name="comparisonType">The <see cref="StringComparison"/> instance to specify culture and case rules</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, char value, StringComparison comparisonType) =>
            IndexOf(sb, value.ToString(), comparisonType);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The character to find</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, char value, int startIndex) =>
            IndexOf(sb, value.ToString(), startIndex);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, string value) =>
            IndexOf(sb, value, StringComparison.CurrentCulture);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The character to find</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, char value) => IndexOf(sb, value.ToString());

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, string value, int startIndex) =>
            IndexOf(sb, value, startIndex, StringComparison.CurrentCulture);

        /// <summary>
        /// Returns true if the length of the string is zero or one.
        /// </summary>
        /// <param name="sb">The string to be used</param>
        /// <returns>True if the length of the string is zero or one</returns>
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static bool IsZeroOrOne(this StringBuilder sb) => sb.Length is 0 or 1;

        /// <summary>
        /// Returns this instance of the StringBuilder in which a specific occurrence of a specified string in the current instance
        /// is replaced another specified string. 
        /// </summary>
        /// <example>
        /// Given a string that says, "Hello my good good good friend", StringBuilder.ReplaceOccurrence("good", "very good", 3) would
        /// result in, "Hello my good good very good friend".
        /// </example>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="oldValue">The string to be replaced</param>
        /// <param name="newValue">The string to replace an occurrence of <paramref name="oldValue"/></param>
        /// <param name="occurrence">The nth occurrence of the <paramref name="oldValue"/> to replace</param>
        /// <returns>This StringBuilder instance</returns>
        [Beta]
        public static StringBuilder ReplaceOccurrence(this StringBuilder sb, string oldValue, string? newValue,
            int occurrence) => ReplaceOccurrence(sb, oldValue, newValue, occurrence, StringComparison.CurrentCulture);

        /// <summary>
        /// Returns this instance of the StringBuilder in which a specific occurrence of a specified string in the current instance
        /// is replaced another specified string. 
        /// </summary>
        /// <example>
        /// Given a string that says, "Hello my good good good friend", StringBuilder.ReplaceOccurrence("good", "very good", 3) would
        /// result in, "Hello my good good very good friend".
        /// </example>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="oldValue">The string to be replaced</param>
        /// <param name="newValue">The string to replace an occurrence of <paramref name="oldValue"/></param>
        /// <param name="occurrence">The nth occurrence of the <paramref name="oldValue"/> to replace</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="oldValue"/> is searched within this instance</param>
        /// <returns>This StringBuilder instance</returns>
        [Beta]
        public static StringBuilder ReplaceOccurrence(this StringBuilder sb, string oldValue, string? newValue,
            int occurrence, StringComparison comparisonType)
        {
            switch (occurrence)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(occurrence));
                case 0:
                    return sb;
            }

            newValue ??= string.Empty;

            int index = sb.IndexOf(oldValue, 0, comparisonType);
            var cnt = 1; // Start at 1 because we assume above index found a value
            while (index != -1)
            {
                if (cnt++ == occurrence)
                {
                    sb.Replace(oldValue, newValue, index, oldValue.Length);
                    break;
                }

                index += oldValue.Length; // Move to the end of the replacement
                index = sb.IndexOf(oldValue, index, comparisonType);
            }

            return sb;
        }

        /// <summary>
        /// Returns this instance of the StringBuilder in which a specific occurrence of a specified string in the current instance
        /// is replaced another specified string on a integer defined interval. This allows certain occurrences/intervals of text
        /// up to a predefined stop count. This is an overload of <see cref="ReplaceInterval(System.Text.StringBuilder,string,string?,int, int, StringComparison)"/>
        /// <example>
        /// Given the following string, "very very very very very very very", StringBuilder.ReplaceInterval("very", "happy", 2, sb.Length, StringComparison.CurrentCulture)
        /// will replace every second "very" with the word "happy" until the very end of the string. The result would be:
        /// "very happy very happy very happy very"
        /// </example>
        /// <example>
        /// Given the following string, "very very very very very very very", StringBuilder.ReplaceInterval("very", "happy", 2, 3)
        /// will replace every second "very" with the word "happy" until the third occurrence of "very". The result would be:
        /// "very happy very very very very very"
        /// </example>
        /// </summary>
        /// <param name="sb">The StringBuilder Instance</param>
        /// <param name="oldValue">The string to be replaced</param>
        /// <param name="newValue">The string to replace an occurrence of <paramref name="oldValue"/></param>
        /// <param name="every">The interval the replacement should follow - read as, "Replace every nth occurrence of <paramref name="oldValue"/>"</param>
        /// <returns>This StringBuilder instance</returns>
        [Beta]
        public static StringBuilder ReplaceInterval(this StringBuilder sb, string oldValue, string? newValue, int every)
            => ReplaceInterval(sb, oldValue, newValue, every, sb.Length, StringComparison.CurrentCulture);

        /// <summary>
        /// Returns this instance of the StringBuilder in which a specific occurrence of a specified string in the current instance
        /// is replaced another specified string on a integer defined interval. This allows certain occurrences/intervals of text
        /// up to a predefined stop count. This is an overload of <see cref="ReplaceInterval(System.Text.StringBuilder,string,string?,int, int, StringComparison)"/>
        /// <example>
        /// Given the following string, "very very very very very very very", StringBuilder.ReplaceInterval("very", "happy", 2, sb.Length, StringComparison.CurrentCulture)
        /// will replace every second "very" with the word "happy" until the very end of the string. The result would be:
        /// "very happy very happy very happy very"
        /// </example>
        /// <example>
        /// Given the following string, "very very very very very very very", StringBuilder.ReplaceInterval("very", "happy", 2, 3)
        /// will replace every second "very" with the word "happy" until the third occurrence of "very". The result would be:
        /// "very happy very very very very very"
        /// </example>
        /// </summary>
        /// <param name="sb">The StringBuilder Instance</param>
        /// <param name="oldValue">The string to be replaced</param>
        /// <param name="newValue">The string to replace an occurrence of <paramref name="oldValue"/></param>
        /// <param name="every">The interval the replacement should follow - read as, "Replace every nth occurrence of <paramref name="oldValue"/>"</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="oldValue"/> is searched within this instance</param>
        /// <returns>This StringBuilder instance</returns>
        [Beta]
        public static StringBuilder ReplaceInterval(this StringBuilder sb, string oldValue, string? newValue, int every,
            StringComparison comparisonType)
            => ReplaceInterval(sb, oldValue, newValue, every, sb.Length, comparisonType);

        /// <summary>
        /// Returns this instance of the StringBuilder in which a specific occurrence of a specified string in the current instance
        /// is replaced another specified string on a integer defined interval. This allows certain occurrences/intervals of text
        /// up to a predefined stop count. This is an overload of <see cref="ReplaceInterval(System.Text.StringBuilder,string,string?,int, int, StringComparison)"/>
        /// <example>
        /// Given the following string, "very very very very very very very", StringBuilder.ReplaceInterval("very", "happy", 2, sb.Length, StringComparison.CurrentCulture)
        /// will replace every second "very" with the word "happy" until the very end of the string. The result would be:
        /// "very happy very happy very happy very"
        /// </example>
        /// <example>
        /// Given the following string, "very very very very very very very", StringBuilder.ReplaceInterval("very", "happy", 2, 3)
        /// will replace every second "very" with the word "happy" until the third occurrence of "very". The result would be:
        /// "very happy very very very very very"
        /// </example>
        /// </summary>
        /// <param name="sb">The StringBuilder Instance</param>
        /// <param name="oldValue">The string to be replaced</param>
        /// <param name="newValue">The string to replace an occurrence of <paramref name="oldValue"/></param>
        /// <param name="every">The interval the replacement should follow - read as, "Replace every nth occurrence of <paramref name="oldValue"/>"</param>
        /// <param name="stop">The number of found instances of <paramref name="oldValue"/></param> to seach before stopping
        /// <returns>This StringBuilder instance</returns>
        [Beta]
        public static StringBuilder ReplaceInterval(this StringBuilder sb, string oldValue, string? newValue, int every,
            int stop)
            => ReplaceInterval(sb, oldValue, newValue, every, stop, StringComparison.CurrentCulture);

        /// <summary>
        /// Returns this instance of the StringBuilder in which a specific occurrence of a specified string in the current instance
        /// is replaced another specified string on a integer defined interval. This allows certain occurrences/intervals of text
        /// up to a predefined stop count.
        /// <example>
        /// Given the following string, "very very very very very very very", StringBuilder.ReplaceInterval("very", "happy", 2, sb.Length, StringComparison.CurrentCulture)
        /// will replace every second "very" with the word "happy" until the very end of the string. The result would be:
        /// "very happy very happy very happy very"
        /// </example>
        /// <example>
        /// Given the following string, "very very very very very very very", StringBuilder.ReplaceInterval("very", "happy", 2, 3)
        /// will replace every second "very" with the word "happy" until the third occurrence of "very". The result would be:
        /// "very happy very very very very very"
        /// </example>
        /// </summary>
        /// <param name="sb">The StringBuilder Instance</param>
        /// <param name="oldValue">The string to be replaced</param>
        /// <param name="newValue">The string to replace an occurrence of <paramref name="oldValue"/></param>
        /// <param name="every">The interval the replacement should follow - read as, "Replace every nth occurrence of <paramref name="oldValue"/>"</param>
        /// <param name="stop">The number of found instances of <paramref name="oldValue"/></param> to seach before stopping
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="oldValue"/> is searched within this instance</param>
        /// <returns>This StringBuilder instance</returns>
        [Beta]
        public static StringBuilder ReplaceInterval(this StringBuilder sb, string oldValue, string? newValue, int every,
            int stop, StringComparison comparisonType)
        {
            if (every < 0)
                throw new ArgumentOutOfRangeException(nameof(every));

            if (stop > oldValue.Length)
                throw new ArgumentOutOfRangeException(nameof(stop));

            newValue ??= string.Empty;

            int index = sb.IndexOf(oldValue, 0, comparisonType);
            var cnt = 1; // Start at 1 because we assume above index found a value
            while (index != -1)
            {
                if (cnt % every == 0)
                {
                    sb.Replace(oldValue, newValue, index, oldValue.Length);
                }

                if (cnt == stop)
                {
                    break;
                }

                index += oldValue.Length; // Move to the end of the replacement
                index = sb.IndexOf(oldValue, index, comparisonType);
                cnt++;
            }

            return sb;
        }

        /// <summary>
        /// Performs a Substring given a starting and ending index, similar to Java.
        /// The operation is performed mathematically as [startIndex, endIndex).
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="startIndex">The inclusive starting index of <paramref name="sb"/></param>
        /// <param name="endIndex">The exclusive ending index of <paramref name="sb"/></param>
        /// <returns>A string that is equivalent to the substring that begins at startIndex in this 
        /// instance, or Empty if startIndex is equal to the length of this instance.</returns>
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string Slice(this StringBuilder sb, int startIndex, int endIndex)
        {
            sb = sb ?? throw new ArgumentNullException(nameof(sb));
            return sb.ToString(startIndex, (endIndex - startIndex) + 1);
        }
    }
}