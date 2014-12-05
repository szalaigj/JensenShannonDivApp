using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace JensenShannonDivComp.Utils
{
    /// <summary>
    /// This implementation is based on the article:
    ///     Grosse, I., Bernaola-Galvan, P., Carpena, P., Roman-Roldan, R., Oliver, J., & Stanley, H. E. (2002). 
    ///     Analysis of symbolic sequences using the Jensen-Shannon divergence.
    ///     Physical Review E, 65(4), 041905
    /// </summary>
    public class Splitter
    {
        private FrequencyComputer frequencyComputer;
        private JenShaDivComputer jenShaDivComputer;
        private double betaParam;
        private double aParam;
        private double bParam;
        private double significanceThreshold;
        private int minSeqLength;
        private ChiSquared chiSquared;

        public Splitter(FrequencyComputer frequencyComputer, JenShaDivComputer jenShaDivComputer)
        {
            this.frequencyComputer = frequencyComputer;
            this.jenShaDivComputer = jenShaDivComputer;
            // The following values are referred to in article (Table 1).
            int k = frequencyComputer.Symbols.Length;
            if (k == 2)
            {
                this.betaParam = 0.8;
                this.aParam = 2.96;
                this.bParam = -7.88;
            }
            else if (k == 4)
            {
                this.betaParam = 0.8;
                this.aParam = 2.44;
                this.bParam = -6.15;
            }
            else if (k == 12)
            {
                this.betaParam = 0.85;
                this.aParam = 2.32;
                this.bParam = -4.32;
            }
            this.chiSquared = new MathNet.Numerics.Distributions.ChiSquared(k - 1);
            this.significanceThreshold = 0.9;
            this.minSeqLength = 5;
        }

        public Splitter(FrequencyComputer frequencyComputer, JenShaDivComputer jenShaDivComputer,
            double betaParam, double aParam, double bParam, double significanceThreshold, int minSeqLength)
        {
            this.frequencyComputer = frequencyComputer;
            this.jenShaDivComputer = jenShaDivComputer;
            this.betaParam = betaParam;
            this.aParam = aParam;
            this.bParam = bParam;
            this.significanceThreshold = significanceThreshold;
            this.minSeqLength = minSeqLength;
        }

        public List<string> split(string sequence)
        {
            List<string> result;
            double? maxDivergence = null;
            string seqPrefixForMax = "";
            string seqPostfixForMax = "";
            if (sequence.Length >= minSeqLength)
            {
                for (int pos = 1; pos < sequence.Length; pos++)
                {
                    computeDivergenceForPos(sequence, ref maxDivergence, ref seqPrefixForMax, ref seqPostfixForMax, pos);
                }    
            }
            double significance = computeSignificance(sequence, maxDivergence);
            result = checkSignificance(sequence, seqPrefixForMax, seqPostfixForMax, significance);
            return result;
        }

        private void computeDivergenceForPos(string sequence, ref double? maxDivergence, ref string seqPrefixForMax, ref string seqPostfixForMax, int pos)
        {
            string sequencePrefix = sequence.Substring(0, pos);
            string sequencePostfix = sequence.Substring(pos);
            double[] frequenciesPrefix = frequencyComputer.computeFrequency(sequencePrefix);
            double[] frequenciesPostfix = frequencyComputer.computeFrequency(sequencePostfix);
            double weightPrefix = (double)sequencePrefix.Length / (double)sequence.Length;
            double weightPostfix = (double)sequencePostfix.Length / (double)sequence.Length;
            double divergence = jenShaDivComputer.computeDivergence(frequenciesPrefix, frequenciesPostfix,
                weightPrefix, weightPostfix);
            if ((maxDivergence == null) || (maxDivergence < divergence))
            {
                maxDivergence = divergence;
                seqPrefixForMax = sequencePrefix;
                seqPostfixForMax = sequencePostfix;
            }
        }

        private double computeSignificance(string sequence, double? maxDivergence)
        {
            double significance = 0.0;
            // If 2 * this.cursorStart > sequence.Length then the maxDivergence has had null value so the following is necessary:
            if (maxDivergence.HasValue && (maxDivergence.Value > 0.0))
            {
                int N = sequence.Length;
                double NEff = aParam * Math.Log(N) + bParam;
                significance = Math.Pow(chiSquared.CumulativeDistribution(N * Math.Log(2) * betaParam * maxDivergence.Value), NEff);
            }
            return significance;
        }

        private List<string> checkSignificance(string sequence, string seqPrefixForMax, string seqPostfixForMax, double significance)
        {
            List<string> result;
            if (significance > significanceThreshold)
            {
                var resultPrefix = split(seqPrefixForMax);
                var resultPostfix = split(seqPostfixForMax);
                result = new List<string>(resultPrefix);
                result.AddRange(resultPostfix);
            }
            else
            {
                result = new List<string>() { sequence };
            }
            return result;
        }
    }
}
