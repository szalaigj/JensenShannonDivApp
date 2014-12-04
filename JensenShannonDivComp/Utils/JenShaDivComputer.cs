using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JensenShannonDivComp.Utils
{
    public class JenShaDivComputer
    {
        private ShannonEntropyComputer entropyComputer;

        public JenShaDivComputer(ShannonEntropyComputer entropyComputer)
        {
            this.entropyComputer = entropyComputer;
        }

        public double computeDivergence(double[] frequencies1, double[] frequencies2, double weight1, double weight2)
        {
            if (frequencies1.Length != frequencies2.Length)
            {
                throw new ArgumentException("The two frequency inputs have not same length.");
            }
            double[] weightedFrequencies1 = computeWeightedFrequencies(frequencies1, weight1);
            double[] weightedFrequencies2 = computeWeightedFrequencies(frequencies2, weight2);
            double[] sumOfFrequencies = computeSum(weightedFrequencies1, weightedFrequencies2);
            return entropyComputer.computeEntropy(sumOfFrequencies)
                - weight1 * entropyComputer.computeEntropy(frequencies1)
                - weight2 * entropyComputer.computeEntropy(frequencies2);
        }

        private double[] computeSum(double[] frequencies1, double[] frequencies2)
        {
            double[] sumOfFrequencies = new double[frequencies1.Length];
            for (int idx = 0; idx < frequencies1.Length; idx++)
            {
                sumOfFrequencies[idx] = frequencies1[idx] + frequencies2[idx];
            }
            return sumOfFrequencies;
        }

        private double[] computeWeightedFrequencies(double[] frequencies, double weight)
        {
            double[] weightedFrequencies = new double[frequencies.Length];
            for (int idx = 0; idx < frequencies.Length; idx++)
            {
                weightedFrequencies[idx] = frequencies[idx] * weight;
            }
            return weightedFrequencies;
        }
    }
}
