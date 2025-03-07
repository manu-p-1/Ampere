﻿using Ampere.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ampere.Enumerable
{
    /// <summary>
    /// A static utility class for .NET <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableUtils
    {
        private static readonly System.Globalization.CultureInfo CulInv = System.Globalization.CultureInfo.InvariantCulture;

        /// <summary>
        /// Concatenates all IEnumerables which are specified in the parameter. The
        /// concatenation occurs in the order specified in the parameter.
        /// </summary>
        /// <typeparam name="T">The enumerable type to be used</typeparam>
        /// <param name="ie">An enumerable of all one dimensional arrays to be concatenated</param>
        /// <exception cref="ArgumentNullException"> Is thrown if any enumerable, which is a candidate to be concatenated, is null</exception>
        /// <returns>A single enumerable with all the concatenated elements</returns>
        /// <example>This simple example shows how to call the <see cref="Fuse{T}"/> method.</example>
        /// <code>
        ///
        /// using static Utilities.EnumerableUtils;
        ///
        /// class TestClass
        /// {
        ///    static void Main(string[] args)
        ///    {
        ///        int[] x = { 1, 2, 3, 4 };
        ///        int[] y = { 1, 2, 3, 4, 5, 6 };
        ///        int[] z = { 1, 2, 3 };
        ///        int[] comb = Fuse(x, y, z).toArray();
        ///        //Printing out 'comb' results in 1, 2, 3, 4, 1, 2, 3, 4, 5, 6, 1, 2, 3
        ///    }
        /// }
        /// </code>
        public static IEnumerable<T> Fuse<T>(params IEnumerable<T>[] ie) //Passing a variable number of IEnumerables as params
        {
            if (ie is null) throw new ArgumentNullException(nameof(ie));
            foreach (var x in ie)
            {
                if (x is null)
                    throw new ArgumentNullException(nameof(x), "One of the params array's were null");
            }

            var arrTotal = ie.Sum(t => t.Count());
            var z = new T[arrTotal];

            var dest = 0;
            foreach (var t in ie)
            {
                var enumerable = t as T[] ?? t.ToArray();
                Array.ConstrainedCopy(enumerable.ToArray(), 0, z, dest, enumerable.Length);
                dest += enumerable.Length;
            }
            return z;
        }

        /// <summary>
        /// Extends a generic array buffer by a specified size given a predefined condition
        /// is satisfied.
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="buf">The buffer to be used</param>
        /// <param name="condition">The condition that should be satisfied before Array resizing</param>
        /// <param name="size">The size in which to resize the buffer if the condition is satisfied</param>
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void CheckBufferAndExtend<T>(ref T[] buf, bool condition, int size)
        {
            if (condition)
                Array.Resize(ref buf, buf.Length + size);
        }

        /// <summary>
        /// Extends an <see cref="IEnumerable{T}"/> buffer by a specified size given a predefined condition
        /// is satisfied.
        /// </summary>
        /// <typeparam name="T">The type of the IEnumerable</typeparam>
        /// <param name="buf">The buffer to be used</param>
        /// <param name="condition">The condition that should be satisfied before Array resizing</param>
        /// <param name="size">The size in which to resize the buffer if the condition is satisfied</param>
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void CheckBufferAndExtend<T>(ref IEnumerable<T> buf, bool condition, int size)
        {
            var x = buf as T[] ?? buf.ToArray();
            if (condition)
                Array.Resize(ref x, x.Length + size);
        }

        /// <summary>
        /// Inserts the specified element at the specified index in the generic array (modifying the original array).
        /// If element at that position exits, If shifts that element and any subsequent elements to the right,
        /// adding one to their indices. The method also allows for inserting more than one element into
        /// the array at one time given that they are specified. This Insert method is functionally similar
        /// to the Insert method of the IList interface. <see cref="System.Collections.IList.Insert(int, object)"/>
        /// for information about the add method of the IList interface. This function also has an enumerable overload.
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="src">The generic array to be used</param>
        /// <param name="startIdx">The index to start insertion</param>
        /// <param name="amtToIns">The amount of elements to insert into the array</param>
        /// <param name="valuesToIns">Optionally, the values to insert into the empty indices of the new array</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when the valuesToIns array does not match the amount to insert (if it is greater than 0)</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when the amtToIns or the startIdx is less than 0</exception>
        /// <example>This sample shows how to call the <see cref="Insert{T}(ref T[], int, int, T[])"/> method.</example>
        /// <seealso cref="System.Collections.IList.Insert(int, object)"/>
        /// <code>
        ///
        /// using static Utilities.EnumerableUtils;
        ///
        /// class TestClass
        /// {
        ///     static void Main(string[] args)
        ///     {
        ///         var w = new int[9] {2, 3, 4, 5, 6, 7, 8, 9, 10};
        ///         Insert(ref w, 1, 3);
        ///         //Printing out 'w' results in: 2, 0, 0, 0, 3, 4, 5, 6, 7, 8, 9, 10
        ///
        ///         var y = new int[9] {2, 3, 4, 5, 6, 7, 8, 9, 10};
        ///         Insert(ref y, 1, 3, 250, 350, 450);
        ///         //Printing out 'y' results in: 2, 250, 350, 450, 3, 4, 5, 6, 7, 8, 9, 10
        ///     }
        /// }
        /// </code>
        public static void Insert<T>(ref T[] src, int startIdx, int amtToIns, params T[] valuesToIns)
        {
            var len = src.Length;

            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }
            if (valuesToIns is null)
            {
                throw new ArgumentNullException(nameof(valuesToIns));
            }
            if (startIdx < 0 || startIdx >= len || amtToIns < 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (amtToIns != valuesToIns.Length && valuesToIns.Length != 0)
            {
                throw new IndexOutOfRangeException("offset amount should equal the number of values to be filled");
            }
            var arrManaged = new T[len + amtToIns];

            if (amtToIns != 0)
            {
                if (startIdx == len - 1)
                {
                    Array.ConstrainedCopy(src, 0, arrManaged, 0, len);
                    Array.ConstrainedCopy(valuesToIns, 0, arrManaged, startIdx + 1, valuesToIns.Length);
                }
                else
                {
                    Array.ConstrainedCopy(src, 0, arrManaged, 0, startIdx);
                    Array.ConstrainedCopy(valuesToIns, 0, arrManaged, startIdx, valuesToIns.Length);
                    Array.ConstrainedCopy(src, startIdx, arrManaged, startIdx + amtToIns, len - startIdx);
                }
            }
            src = arrManaged;
        }

        /// <summary>
        /// Inserts the specified element at the specified index in the enumerable (modifying the original enumerable).
        /// If element at that position exits, If shifts that element and any subsequent elements to the right,
        /// adding one to their indices. The method also allows for inserting more than one element into
        /// the enumerable at one time given that they are specified. This Insert method is functionally similar
        /// to the Insert method of the IList interface. <see cref="System.Collections.IList.Insert(int, object)"/>
        /// for information about the add method of the IList interface. This function also has a generic array overload.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="src">The IEnumerable to be used</param>
        /// <param name="startIdx">The index to start insertion</param>
        /// <param name="amtToIns">The amount of elements to insert into the enumerable</param>
        /// <param name="valuesToIns">Optionally, the values to insert into the empty indices of the new enumerable</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when the valuesToIns enumerable does not match the amount to insert (if it is greater than 0)</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when the amtToIns or the startIdx is less than 0</exception>
        /// <example>This sample shows how to call the <see cref="Insert{T}(ref IEnumerable{T}, int, int, T[])"/> method.</example>
        /// <seealso cref="System.Collections.IList.Insert(int, object)"/>
        /// <code>
        ///
        /// using static Utilities.EnumerableUtils;
        ///
        /// class TestClass
        /// {
        ///     static void Main(string[] args)
        ///     {
        ///         var w = new int[9] {2, 3, 4, 5, 6, 7, 8, 9, 10}.AsEnumerable();
        ///         Insert(ref w, 1, 3);
        ///         //Printing out 'w' results in: 2, 0, 0, 0, 3, 4, 5, 6, 7, 8, 9, 10
        ///
        ///         var y = new int[9] {2, 3, 4, 5, 6, 7, 8, 9, 10}.AsEnumerable();
        ///         Insert(ref y, 1, 3, 250, 350, 450);
        ///         //Printing out 'y' results in: 2, 250, 350, 450, 3, 4, 5, 6, 7, 8, 9, 10
        ///     }
        /// }
        /// </code>
        public static void Insert<T>(ref IEnumerable<T> src, int startIdx, int amtToIns, params T[] valuesToIns)
        {
            var enumerable = src as T[] ?? src.ToArray();
            Insert(ref enumerable, startIdx, amtToIns, valuesToIns);
        }

        /// <summary>
        /// Given a primary enumerable to check against and one or more test enumerable's, InnerContains executes an intersection
        /// of the primary and test enumerables to verify if all values in the test are contained within the primary enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the IEnumerable</typeparam>
        /// <param name="ie">The primary IEnumerable to check against</param>
        /// <param name="isAll">Indicates whether to check whether all values are intersected or partially intersected</param>
        /// <param name="otherArrays">An array of Enumerable's to test against the primary enumerable</param>
        /// <returns>An instance of the <see cref="InnerContainsProgram{T}"/> to verify the result and violating enumerables</returns>
        public static InnerContainsProgram<T> InnerContains<T>(this IEnumerable<T> ie, bool isAll, params IEnumerable<T>[] otherArrays)
        {
            ArgumentNullException.ThrowIfNull(ie);

            ArgumentNullException.ThrowIfNull(otherArrays);

            return new InnerContainsProgram<T>(ie, isAll, otherArrays);
        }

        /// <summary>
        /// Returns whether an IEnumerable is null or empty
        /// </summary>
        /// <typeparam name="T">The type of the IEnumerable</typeparam>
        /// <param name="ie">The IEnumerable to be used</param>
        /// <returns>The truth</returns>
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> ie) => ie is null || !ie.Any();

        /// <summary>
        /// Returns true if the Count of the IEnumerable is zero or one.
        /// </summary>
        /// <typeparam name="T">The type of the IEnumerable</typeparam>
        /// <param name="ie">The IEnumerable to be used</param>
        /// <returns>True if the Count of the IEnumerable is zero or one</returns>
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static bool IsZeroOrOne<T>(this IEnumerable<T> ie)
        {
            ie = ie ?? throw new ArgumentNullException(nameof(ie));
            var cnt = ie.Count();
            return cnt is 0 or 1;
        }

        /// <summary>
        /// Enables python style for-loop for easier readability. This loop begins
        /// at the starting value and loops until the end - 1,
        /// </summary>
        /// <param name="start">The starting counter for the loop (inclusive)</param>
        /// <param name="end">The ending counter for the loop (exclusive)</param>
        /// <returns>An IEnumerable representing the current index</returns>
        /// <example>This example shows how to use the <see cref="Range(int, int)"/>method.</example>
        /// <code>
        /// using static Utilities.EnumerableUtils;
        ///
        /// class TestClass
        /// {
        ///     static void Main(string[] args)
        ///     {
        ///         foreach(int i in Range(0,10)) Console.WriteLine(i); //Prints 0 - 9
        ///     }
        /// }
        /// </code>
        public static IEnumerable<int> Range(int start, int end)
        {
            for (var i = start; i < end; i++)
                yield return i;
        }

        /// <summary>
        /// Cryptographically shuffles an enumerable. 
        /// </summary>
        /// <typeparam name="T">The element type of the IEnumerable</typeparam>
        /// <param name="src">The IEnumerable</param>
        /// <returns>The Shuffled IEnumerable</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> src)
        {
            src = src ?? throw new ArgumentNullException(nameof(src));
            var enumerable = src as T[] ?? src.ToArray();

            if (enumerable.IsZeroOrOne())
            {
                return enumerable;
            }

            return src.GetType() == typeof(Array)
                ? Shuffler.Shuffle((T[])src)
                : Shuffler.Shuffle(enumerable.ToArray());
        }

        /// <summary>
        /// Enables python style for-loop for easier readability. This loop begins
        /// at the starting value and loops until the end.
        /// </summary>
        /// <param name="start">The starting counter for the loop (inclusive)</param>
        /// <param name="end">The ending counter for the loop (inclusive)</param>
        /// <returns>An IEnumerable representing the current index</returns>
        /// <example>This example shows how to use the <see cref="Span(int, int)"/>method.</example>
        /// <code>
        /// using static Utilities.EnumerableUtils;
        ///
        /// class TestClass
        /// {
        ///     static void Main(string[] args)
        ///     {
        ///         foreach(int i in Span(0,10)) Console.WriteLine(i); //Prints 0 - 10
        ///     }
        /// }
        /// </code>
        public static IEnumerable<int> Span(int start, int end)
        {
            for (var i = start; i <= end; i++)
                yield return i;
        }

        /// <summary>
        /// Prints a string representation of an enumerable. There are 4 supported lengths for the fmtExp. The
        /// default length is 0 and the default behavior depends on the type of the enumerable. If the type is primitive
        /// (based on the System.IsPrimitive property) including decimal and string, then it prints the enumerable with a space
        /// as a separator between each element. If the enumerable is not primitive, it prints the enumerable with no separator.
        /// A fmtExp of length 1 specifies a character to separate each element. The enumerable is printed out, following
        /// a default behavior, except with the specified separator rather than the default separator. A fmtExp
        /// of length 2 specifies two characters to mark the left and right outer bounds of the enumerable, A fmtExp
        /// of length 3 specifies a character for the left outer bound of the enumerable, followed by a separator character,
        /// followed by a character for the right outer bound of the enumerable. If no separator is desired, the "/0+" expression
        /// can be specified.The evenlySpacedSeparator parameter specifies whether an even number of spaces should be on
        /// both sides of the separator. This parameter ignores Object type enumerables excluding decimal and string.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="src">The IEnumerable to be used</param>
        /// <param name="fmtExp">The defined expression to be optionally used</param>
        /// <param name="evenlySpacedSeparator">Determines whether the spacing between each element should be the same</param>
        /// <returns>The string representation of the enumerable</returns>
        /// <exception cref="ArgumentNullException">If arr is null</exception>
        /// <exception cref="FormatException">If the formatting expression length is neither 0 nor 3</exception>
        /// <example>This sample shows how to call the <see cref="ToString{T}"/> method.</example>
        /// <code>
        ///
        /// using static Utilities.EnumerableUtils;
        ///
        /// class TestClass
        /// {
        ///     static void Main(string[] args)
        ///     {
        ///         var w = new int[9] {2, 3, 4, 5, 6, 7, 8, 9, 10};
        ///         Console.WriteLine(w.ToString("[,]"));
        ///         //Printing out 'w' results in: [2, 3, 4, 5, 6, 7, 8, 9, 10]
        ///
        ///         var x = new int[9] {2, 3, 4, 5, 6, 7, 8, 9, 10};
        ///         Console.WriteLine(x.ToString("(|)", true));
        ///         //Printing out 'x' results in: (2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10)
        ///
        ///         var y = new string[4] {"Bill", "Bob", "Tom", "Joe"};
        ///         Console.WriteLine(y.ToString());
        ///         //Printing out 'y' results in: Bill Bob Tom Joe
        ///     }
        /// }
        /// </code>
        public static string ToString<T>(this IEnumerable<T> src, string fmtExp = "", bool evenlySpacedSeparator = false)
        {
            fmtExp = fmtExp ?? throw new ArgumentNullException(nameof(fmtExp));
            var frl = fmtExp.Length;

            src = src ?? throw new ArgumentNullException(nameof(src));

            if (frl > 3)
            {
                throw new FormatException("Unsupported Expression");
            }

            string outerLeft = string.Empty, separator = string.Empty, outerRight = string.Empty;
            var hasNoSep = false;
            if (fmtExp.Equals("/0+", StringComparison.InvariantCulture))
            {
                hasNoSep = true;
                frl = 1;
            }

            switch (frl)
            {
                case 3:
                    outerLeft = fmtExp[0].ToString(CulInv);
                    separator = fmtExp[1].ToString(CulInv);
                    outerRight = fmtExp[2].ToString(CulInv);
                    break;

                case 2:
                    outerLeft = fmtExp[0].ToString(CulInv);
                    outerRight = fmtExp[1].ToString(CulInv);
                    break;

                case 1:
                    separator = fmtExp[0].ToString(CulInv);
                    break;
            }

            var isLooselyPrimitive = false;
            var type = typeof(T);
            if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
            {
                isLooselyPrimitive = true;
            }

            var sb = new StringBuilder();

            sb.Append(outerLeft);

            var enumerable = src as T[] ?? src.ToArray();
            var len = enumerable.Length;

            for (var i = 0; i < len; i++)
            {
                if (i == len - 1)
                {
                    sb.Append(enumerable.ElementAt(i));
                }
                else switch (frl)
                {
                    case 0:
                    case 2:
                    case 3:
                        defBehavior:
                        if (evenlySpacedSeparator && isLooselyPrimitive)
                        {
                            if (frl != 2)
                            {
                                sb.Append(enumerable.ElementAt(i) + " " + separator + " ");
                            }
                            else
                            {
                                sb.Append(enumerable.ElementAt(i) + separator + " ");
                            }
                        }
                        else if (isLooselyPrimitive)
                        {
                            sb.Append(enumerable.ElementAt(i) + separator + " ");
                        }
                        else
                        {
                            sb.Append(enumerable.ElementAt(i) + separator);
                        }
                        break;

                    case 1:
                        if (hasNoSep)
                            sb.Append(enumerable.ElementAt(i));
                        else
                            goto defBehavior;
                        break;
                }
            }
            sb.Append(outerRight);
            return sb.ToString();
        }

    }
}
