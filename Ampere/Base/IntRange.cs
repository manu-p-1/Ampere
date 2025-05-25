using System;
using System.Collections;
using System.Collections.Generic;

namespace Ampere.Base
{
    /// <summary>
    /// The range class represents a range of int values. Unlike other <see cref="IRangify{T}"/> implementing classes,
    /// IntRange contains an <see cref="IEnumerator{T}"/> of type int to enumerate through all the values between
    /// the minimum and maximum ranges. By convention, both sides of the range should be inclusive values.
    /// </summary>
    public class IntRange : Range<int>, IEnumerable<int>
    {
        /// <summary>
        /// Creates a new instance of IntRange, specifying the minimum and maximum values.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public IntRange(int minimum, int maximum) : base(minimum, maximum) { }

        /// <summary>
        /// Returns an instance of the IntRangeEnumerator that's used to enumerate through the range
        /// values of this instance.
        /// </summary>
        /// <returns>An instance of the IntRangeEnumerator class</returns>
        public IEnumerator<int> GetEnumerator()
        {
            for (int i = Minimum; i <= Maximum; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Returns an instance of the IntRangeEnumerator that's used to enumerate through the range
        /// values of this instance.
        /// </summary>
        /// <returns>An instance of the IntRangeEnumerator class</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}