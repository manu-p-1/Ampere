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
        private readonly IEnumerable<T> baseArray;

        /// <summary>
        /// Indicates whether to check whether all values are intersected or partially intersected
        /// </summary>
        private readonly bool isAll;

        /// <summary>
        /// A list of IEnumberables to hold constructor params
        /// </summary>
        private readonly IEnumerable<T>[] otherArrays;

        /// <summary>
        /// The IEnumerable that was 
        /// </summary>
        private IEnumerable<T> violatedEnumerable;

        /// <summary>
        /// The Constructor to create a new instance of the InnerContainsProgram
        /// </summary>
        /// <param name="baseArray">The IEnumberable to check against</param>
        /// <param name="otherArrays">The list of IEnumberables to check against the baseArray</param>
        public InnerContainsProgram(IEnumerable<T> baseArray, bool isAll, params IEnumerable<T>[] otherArrays)
        {
            this.baseArray = baseArray;
            this.otherArrays = otherArrays;
            this.isAll = isAll;
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
            if (isAll)
            {
                foreach (IEnumerable<T> x in otherArrays)
                {
                    if (x.Intersect(baseArray).Count() != x.Count())
                    {
                        this.violatedEnumerable = x;
                        return false;
                    }

                }
            }
            else
            {
                foreach (IEnumerable<T> x in otherArrays)
                {
                    if (!x.Intersect(baseArray).Any())
                    {
                        this.violatedEnumerable = x;
                        return false;
                    }

                }
            }
            return true;
        }

        /// <summary>
        /// A property to identify which enumerable violated the containing condition. This method
        /// returns null unless <see cref="CheckContains"/> is executed.
        /// </summary>
        public IEnumerable<T> ViolatedEnumerable { get => violatedEnumerable; }
    }
}
