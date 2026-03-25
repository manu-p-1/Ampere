using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace Ampere.Base;

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
    /// <param name="minimum">The minimum value (inclusive)</param>
    /// <param name="maximum">The maximum value (inclusive)</param>
    public IntRange(int minimum, int maximum) : base(minimum, maximum) { }

    /// <summary>
    /// Returns an instance of the IntRangeEnumerator that enumerates through the range
    /// values of this instance (inclusive on both ends).
    /// </summary>
    /// <returns>An instance of the IntRangeEnumerator class</returns>
    public IEnumerator<int> GetEnumerator() => new IntRangeEnumerator(Minimum, Maximum);

    /// <summary>
    /// Returns an instance of the IntRangeEnumerator that enumerates through the range
    /// values of this instance.
    /// </summary>
    /// <returns>An instance of the IntRangeEnumerator class</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class IntRangeEnumerator : IEnumerator<int>
    {
        private readonly int _minimum;
        private readonly int _maximum;
        private int _position;

        public IntRangeEnumerator(int minimum, int maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
            _position = minimum - 1;
        }

        public bool MoveNext()
        {
            _position++;
            return _position <= _maximum;
        }

        public void Reset()
        {
            _position = _minimum - 1;
        }

        public int Current
        {
            get
            {
                if (_position < _minimum || _position > _maximum)
                    throw new InvalidOperationException("Enumerator is not positioned on a valid element.");
                return _position;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }
}
