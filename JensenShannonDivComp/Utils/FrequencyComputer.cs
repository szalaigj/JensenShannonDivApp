using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JensenShannonDivComp.Utils
{
    public class FrequencyComputer
    {
        private char[] symbols;

        public char[] Symbols
        {
            get { return symbols; }
        }

        public FrequencyComputer(char[] symbols)
        {
            this.symbols = symbols;
        }

        public double[] computeFrequency(string sequence)
        {
            double[] frequencies = new double[symbols.Length];
            Dictionary<char, long> chrCounts = InitChrCountsDict();
            CountCharsInSeq(sequence, chrCounts);
            DetermineFreqList(sequence, frequencies, chrCounts);
            return frequencies;
        }

        private Dictionary<char, long> InitChrCountsDict()
        {
            Dictionary<char, long> chrCounts = new Dictionary<char, long>();
            foreach (var symbol in symbols)
            {
                chrCounts.Add(symbol, 0);
            }
            return chrCounts;
        }

        private static void CountCharsInSeq(string sequence, Dictionary<char, long> chrCounts)
        {
            foreach (var chr in sequence)
            {
                if (!chrCounts.ContainsKey(chr))
                {
                    throw new ArgumentException("The input sequence contains such symbol which is not valid.");
                }
                else
                {
                    chrCounts[chr]++;
                }
            }
        }

        private void DetermineFreqList(string sequence, double[] frequencies, Dictionary<char, long> chrCounts)
        {
            int idx = 0;
            foreach (var symbol in symbols)
            {
                frequencies[idx] = (double)chrCounts[symbol] / (double)sequence.Length;
                idx++;
            }
        }
    }
}
