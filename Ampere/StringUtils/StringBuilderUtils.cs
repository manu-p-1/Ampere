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
        public static StringBuilder AppendFromEnumerable<T>(this StringBuilder sb, IEnumerable<T> enumerable, Func<T, string> func)
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
        public static StringBuilder AppendLineFromEnumerable<T>(this StringBuilder sb, IEnumerable<T> enumerable, Func<T, string> func)
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
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, int count, StringComparison comparisonType)
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
                    while (index < length && string.Equals(sb[i + index].ToString(), value[index].ToString(), comparisonType))
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
        {
            return IndexOf(sb, value, startIndex, sb.Length - startIndex, comparisonType);
        }

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
        {
            return IndexOf(sb, value.ToString(), startIndex, comparisonType);
        }

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
        {
            return IndexOf(sb, value, 0, sb.Length, comparisonType);
        }

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
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, int count)
        {
            return IndexOf(sb, value, startIndex, count, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The character to find</param>
        /// <param name="comparisonType">The <see cref="StringComparison"/> instance to specify culture and case rules</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, char value, StringComparison comparisonType)
        {
            return IndexOf(sb, value.ToString(), comparisonType);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The character to find</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, char value, int startIndex)
        {
            return IndexOf(sb, value.ToString(), startIndex);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, string value)
        {
            return IndexOf(sb, value, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The character to find</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, char value)
        {
            return IndexOf(sb, value.ToString());
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <returns>The index if found and -1 otherwise</returns>
        [Beta]
        public static int IndexOf(this StringBuilder sb, string value, int startIndex)
        {
            return IndexOf(sb, value, startIndex, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="n"></param>
        [Beta]
        public static StringBuilder ReplaceNth(this StringBuilder sb, string from, string to, int n)
        {
            int index = sb.IndexOf(from, 0);
            var cnt = 1; // Start at 1 because we assume above index found a value
            while (index != -1)
            {
                if (cnt == n)
                {
                    sb.Replace(from, to, index, from.Length);
                    break;
                }
                index += to.Length; // Move to the end of the replacement
                index = sb.IndexOf(from, index);
                cnt++;
            }
            return sb;
        }
    }
}
