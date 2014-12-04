using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JensenShannonDivComp.Utils
{
    public class ShannonEntropyComputer
    {
        public double computeEntropy(double[] frequencies)
        {
            double entropy = 0.0;
            foreach (var frequency in frequencies)
            {
                if (frequency != 0.0)
                {
                    entropy -= frequency * Math.Log(frequency);
                }
            }
            return entropy;
        }
    }
}
