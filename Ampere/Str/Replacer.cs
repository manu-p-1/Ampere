using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ampere.Str
{
    internal class Replacer
    {
        private const short _defaultBufferExtension = 256;

        public string Str { get; }
        public string OldValue { get; }
        public string NewValue { get; }
        public int StartIndex { get; }
        public int Count { get; }
        public StringComparison Comparison { get; }

        internal Replacer(string str, string oldValue, string newValue, int startIndex, int count, StringComparison comparison)
        {
            Str = str;
            OldValue = oldValue;
            NewValue = newValue;
            StartIndex = startIndex;
            Count = count;
            Comparison = comparison;
        }

        internal string ReplaceRange()
        {
            int index = Str.IndexOf(OldValue, StartIndex, Comparison);
            var ctr = 0;
            var oldCtr = 0;
            var buffer = new char[Str.Length];

            char[] pSourceCopy = Str.ToCharArray();
            while (index != -1)
            {
                if (oldCtr == index)
                {
                    if (oldCtr + OldValue.Length > Count)
                    {
                        break;
                    }

                    oldCtr += OldValue.Length;

                    foreach (char c in NewValue)
                    {
                        if (buffer[^1] != default(char))
                        {
                            Array.Resize(ref buffer, buffer.Length + _defaultBufferExtension);
                        }

                        buffer[ctr++] = c;
                    }

                    index += OldValue.Length; // Move to the end of the replacement
                    index = Str.IndexOf(OldValue, index, Comparison);
                }
                else
                {
                    buffer[ctr] = pSourceCopy[oldCtr];
                    //pSourceCopy++;
                    ctr++;
                    oldCtr++;
                }
                
            }
            return new string(buffer, 0, ctr);
        }
    }
}
