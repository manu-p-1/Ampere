using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ampere.Str
{
    internal class LcpFinder
    {
        private IEnumerable<string> Strs { get; }
        private bool IgnoreCase { get; }

        public LcpFinder(IEnumerable<string> strs, bool ignoreCase = false)
        {
            Strs = strs;
            IgnoreCase = ignoreCase;
        }

        public string Find()
        {
            int minLength = FindMin();
            var sb = new StringBuilder();
            string mainWord = Strs.ElementAt(0);

            int low = 0, high = minLength - 1;
            while (low <= high)
            {
                var mid = low + (high - low) / 2;
                if (CheckContain(mainWord, low, mid))
                {
                    sb.Append(mainWord[low..(mid + 1)]);
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return sb.ToString();
        }

        private int FindMin()
        {
            var min = Strs.ElementAt(0).Length;
            return Strs.Select(s => s.Length).Prepend(min).Min();
        }

        private bool CheckContain(string str, int st, int end)
        {
            for (var i = 0; i < Strs.Count(); i++)
            {
                string word = Strs.ElementAt(i);
                if (IgnoreCase)
                {
                    word = word.ToUpperInvariant();
                    str = str.ToUpperInvariant();
                }
                for (int j = st; j <= end; j++)
                {
                    if (word[j] != str[j])
                        return false;
                }
            }
            return true;
        }
    }
}
