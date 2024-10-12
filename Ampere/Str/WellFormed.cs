using System;
using System.Collections.Generic;
using System.Linq;

namespace Ampere.Str
{
    /// <summary>
    /// A utility class that contains functions to determine
    /// whether a string is a well-formed string.
    /// </summary>
    internal class WellFormedUtility
    {
        /// <summary>
        /// An instance of the Dictionary containing this alphabet.
        /// </summary>
        private Dictionary<char, char> Alphabet { get; }

        /// <summary>
        /// constructor that sets up the alphabet
        /// </summary>
        /// <param name="dct">A dictionary representing an alphabet</param>
        public WellFormedUtility(Dictionary<char, char> dct)
        {
            Alphabet = dct ?? throw new ArgumentNullException(nameof(dct));
        }

        /// <summary>
        /// The default alphabet
        /// </summary>
        public static Dictionary<char, char> DefaultAlphabet { get; }
            = new(4)
            {
                { '(', ')' },
                { '{', '}' },
                { '[', ']' },
                { '<', '>' },
            };

        /// <summary>
        /// Verifies if the string is well-formed by using
        /// a stack data structure to measure the balance of the string.
        /// </summary>
        /// <param name="inp">The input string</param>
        /// <returns></returns>
        public bool Run(string inp)
        {
            var stk = new Stack<char>(10);
            try
            {
                foreach (var c in inp.Where(c => Alphabet.ContainsKey(c) || Alphabet.ContainsValue(c)))
                {
                    if (Alphabet.ContainsKey(c)) stk.Push(c);
                    else if (Alphabet[stk.Pop()] != c)
                        return false;
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or NullReferenceException)
            {
                return false;
            }
            return true;
        }
    } //WellFormedUtilities
}
