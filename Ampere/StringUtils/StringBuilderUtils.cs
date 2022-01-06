using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="value">The character to find</param>
        /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
        /// <returns>The index if found and -1 otherwise</returns>
        public static int IndexOf(this StringBuilder sb, char value, bool ignoreCase = true)
        {
            return IndexOf(sb, new [] {value}, 0, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
        /// <returns>The index if found and -1 otherwise</returns>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex, bool ignoreCase = true)
        {
            return IndexOf(sb, new[] { value }, startIndex, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
        /// <returns>The index if found and -1 otherwise</returns>
        public static int IndexOf(this StringBuilder sb, string value, bool ignoreCase = true)
        {
            return IndexOf(sb, value, 0, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
        /// <returns>The index if found and -1 otherwise</returns>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase = true)
        {
            return IndexOf(sb, value.ToCharArray(), startIndex, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a specified string within this StringBuilder isntance.
        /// The method returns -1 if the string is not found in this instance. A new string is not created during the search.
        /// </summary>
        /// <param name="sb">The StringBuilder instance</param>
        /// <param name="value">The string to find as a character array</param>
        /// <param name="startIndex">The starting index of where to search, inclusive</param>
        /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
        /// <returns>The index if found and -1 otherwise</returns>
        public static int IndexOf(this StringBuilder sb, char[] value, int startIndex, bool ignoreCase=true)
        {
            if (value.Length == 0 && startIndex == 0)
            {
                return 0;
            }

            if (startIndex < 0 || value.Length > sb.Length || startIndex >= sb.Length)
            {
                return -1;
            }

            int index;
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;
            
            if (ignoreCase)
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (char.ToLower(sb[i]) == char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (char.ToLower(sb[i + index]) == char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }
                return -1;
            }

            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static StringBuilder ReplaceAll(this StringBuilder sb, string from, string to)
        {
            int index = sb.IndexOf(from, 0);
            while (index != -1)
            {
                sb.Replace(from, to);
                index += to.Length; // Move to the end of the replacement
                index = sb.IndexOf(from, index);
            }

            return sb;
        }
    }
}
