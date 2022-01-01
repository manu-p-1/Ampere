using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ampere.EnumerableUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InnerContainsProgram<T>
    {
        private readonly IEnumerable<T> baseArray;
        private readonly IEnumerable<T>[] otherArrays;
        private IEnumerable<T> struckEnumerable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseArray"></param>
        /// <param name="otherArrays"></param>
        public InnerContainsProgram(IEnumerable<T> baseArray, params IEnumerable<T>[] otherArrays)
        {
            this.baseArray = baseArray;
            this.otherArrays = otherArrays;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckContains()
        {
            foreach (IEnumerable<T> x in otherArrays)
            {
                if (x.Intersect(baseArray).Count() != x.Count())
                {
                    this.struckEnumerable = x;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<T> StruckEnumberable { get => struckEnumerable; }
    }
}
