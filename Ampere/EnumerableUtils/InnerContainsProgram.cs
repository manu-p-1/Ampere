using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ampere.EnumerableUtils
{
    /// <summary>
    /// Program to check whether all values in one or more enumerables's are included in a
    /// specified enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the IEnumerable</typeparam>
    public class InnerContainsProgram<T>
    {
        /// <summary>
        /// The IEnumerable to check against
        /// </summary>
        private readonly IEnumerable<T> _baseArray;

        /// <summary>
        /// Indicates whether to check whether all values are intersected or partially intersected
        /// </summary>
        private readonly bool _isAll;

        /// <summary>
        /// A list of IEnumberables to hold constructor params
        /// </summary>
        private readonly IEnumerable<T>[] _otherArrays;

        /// <summary>
        /// A property to identify which enumerable violated the containing condition. This method
        /// returns null unless <see cref="CheckContains"/> is executed.
        /// </summary>
        public IEnumerable<T> ViolatedEnumerable { get; private set; }

        /// <summary>
        /// The Constructor to create a new instance of the InnerContainsProgram
        /// </summary>
        /// <param name="baseArray">The IEnumberable to check against</param>
        /// <param name="isAll">Indicates whether to check whether all values are intersected or partially intersected</param>
        /// <param name="otherArrays">The list of IEnumberables to check against the baseArray</param>
        public InnerContainsProgram(IEnumerable<T> baseArray, bool isAll, params IEnumerable<T>[] otherArrays)
        {
            this._baseArray = baseArray;
            this._otherArrays = otherArrays;
            this._isAll = isAll;
        }

        /// <summary>
        /// Checks whether each array specified in otherArrays is completely contained in the
        /// base array. Each value in each array must be contained in the base array for this
        /// method to return true, otherwise, false is returned and the ViolatedEnumerable property
        /// is set with the enumerable that violated the condition. This function runs a LINQ intersection
        /// between each enumerable.
        /// </summary>
        /// <returns>True if each enumerable is contained in the base and false otherwise.</returns>
        public bool CheckContains()
        {
            if (_isAll)
            {
                foreach (var x in _otherArrays)
                {
                    var violatedEnumerable = x as T[] ?? x.ToArray(); // To avoid multi-enumeration in LINQ Query
                    if (violatedEnumerable.Intersect(_baseArray).Count() != violatedEnumerable.Length)
                    {
                        ViolatedEnumerable = violatedEnumerable;
                        return false;
                    }
                }
            }
            else
            {
                foreach (var x in _otherArrays)
                {
                    var violatedEnumerable = x as T[] ?? x.ToArray(); // To avoid multi-enumeration in LINQ Query
                    if (!violatedEnumerable.Intersect(_baseArray).Any())
                    {
                        ViolatedEnumerable = violatedEnumerable;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
