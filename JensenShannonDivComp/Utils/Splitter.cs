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
            this.significanceThreshold = 0.95;
        }

        public Splitter(FrequencyComputer frequencyComputer, JenShaDivComputer jenShaDivComputer,
            double betaParam, double aParam, double bParam, double significanceThreshold)
        {
            this.frequencyComputer = frequencyComputer;
            this.jenShaDivComputer = jenShaDivComputer;
            this.betaParam = betaParam;
            this.aParam = aParam;
            this.bParam = bParam;
            this.significanceThreshold = significanceThreshold;
        }

        public string[] split(string sequence)
        {
            string[] result;
            double? maxDivergence = null;
            string seqPrefixForMax = "";
            string seqPostfixForMax = "";
            for (int pos = 1; pos < sequence.Length; pos++)
            {
                computeDivergenceForPos(sequence, ref maxDivergence, ref seqPrefixForMax, ref seqPostfixForMax, pos);
            }
            int k = frequencyComputer.Symbols.Length;
            int N = sequence.Length;
            double NEff = aParam * Math.Log(N) + bParam;
            var chiSquared = new MathNet.Numerics.Distributions.ChiSquared(k - 1);
            double significance = Math.Pow(chiSquared.CumulativeDistribution(N * Math.Log(2) * betaParam * maxDivergence.Value), NEff);
            if (significance > significanceThreshold)
            {
                result = new string[] { seqPrefixForMax, seqPostfixForMax };
            }
            else
            {
                result = new string[] { };
            }
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
    }
}
