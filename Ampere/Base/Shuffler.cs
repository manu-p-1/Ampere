using System;
using System.Security.Cryptography;

namespace Ampere.Base
{
    /// <summary>
    /// A utility to shuffle an array using the Fisher-Yates algorithm.
    /// </summary>
    public static class Shuffler
    {
        /// <summary>
        /// Shuffles the array using the Fisher-Yates algorithm with cryptographically secure random number generation.
        /// </summary>
        /// <typeparam name="T">The element type of the array.</typeparam>
        /// <param name="data">The array to be shuffled.</param>
        /// <returns>The shuffled array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided array is null.</exception>
        public static T[] Shuffle<T>(T[] data)
        {
            ArgumentNullException.ThrowIfNull(data);

            var len = data.Length;
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];

            for (var i = len - 1; i > 0; i--)
            {
                rng.GetBytes(bytes);
                var j = (int)(BitConverter.ToUInt32(bytes, 0) % (uint)(i + 1));

                (data[i], data[j]) = (data[j], data[i]);
            }

            return data;
        }
    }
}
