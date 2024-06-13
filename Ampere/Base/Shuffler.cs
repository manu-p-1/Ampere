using System;
using System.Security.Cryptography;

namespace Ampere.Base
{
    /// <summary>
    /// A utility to shuffle an array using the Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T">The element type of the array.</typeparam>
    internal sealed class Shuffler<T>
    {
        private readonly T[] _data;
        private readonly RandomNumberGenerator _rng;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shuffler{T}"/> class with the specified array.
        /// </summary>
        /// <param name="data">The array to be shuffled.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided array is null.</exception>
        public Shuffler(T[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _rng = RandomNumberGenerator.Create();
        }

        /// <summary>
        /// Shuffles the array using the Fisher-Yates algorithm with cryptographically secure random number generation.
        /// </summary>
        /// <returns>The shuffled array.</returns>
        public T[] Shuffle()
        {
            var len = _data.Length;
            var bytes = new byte[4];

            for (var i = len - 1; i > 0; i--)
            {
                _rng.GetBytes(bytes);
                var j = (int)(BitConverter.ToUInt32(bytes, 0) % (uint)(i + 1));

                (_data[i], _data[j]) = (_data[j], _data[i]);
            }

            return _data;
        }
    }
}
