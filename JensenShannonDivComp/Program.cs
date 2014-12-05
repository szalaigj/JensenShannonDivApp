using JensenShannonDivComp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JensenShannonDivComp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Please give the symbols as a string in the program argument array.");
            }
            char[] symbols = args[0].ToCharArray();
            FrequencyComputer frequencyComputer = new FrequencyComputer(symbols);
            ShannonEntropyComputer shannonEntropyComputer = new ShannonEntropyComputer();
            JenShaDivComputer jenShaDivComputer = new JenShaDivComputer(shannonEntropyComputer);
            Splitter splitter = new Splitter(frequencyComputer, jenShaDivComputer);
            Console.WriteLine("Please give the action type:");
            Console.WriteLine("\t'seq' - add new sequence");
            Console.WriteLine("\t'load' - load a sequence from a file");
            Console.WriteLine("\t'ent' - query entropy of the current sequence");
            Console.WriteLine("\t'jensha' - query Jensen-Shannon divergence based on position");
            Console.WriteLine("\t'split' - split the sequence based on the maximal Jensen-Shannon divergence values of subsequences");
            Console.WriteLine("\t'save' - save subsequences of the splitted sequence");
            Console.WriteLine("\t'exit' - exit program");
            string sequence = "";
            List<string> subsequences = new List<string>();
            bool done = false;
            while (!done)
            {
                Console.Write("action type>>>");
                string action = Console.ReadLine();
                switch (action)
                {
                    case "seq":
                        sequence = addSequence();
                        break;
                    case "load":
                        sequence = loadSequences();
                        break;
                    case "ent":
                        computeShaEnt(frequencyComputer, shannonEntropyComputer, sequence);
                        break;
                    case "jensha":
                        computeJenShaDiv(frequencyComputer, jenShaDivComputer, sequence);
                        break;
                    case "split":
                        subsequences = splitSequence(splitter, sequence);
                        break;
                    case "save":
                        saveSubsequences(subsequences);
                        break;
                    case "exit":
                        done = true;
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }

        private static string addSequence()
        {
            Console.WriteLine("Please give the sequence:");
            string sequence = Console.ReadLine();
            return sequence;
        }

        private static string loadSequences()
        {
            string sequence = "";
            Console.WriteLine("Please give the path with filename:");
            string file = Console.ReadLine();
            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    sequence += line;
                }
            }
            return sequence;
        }

        private static void computeShaEnt(FrequencyComputer frequencyComputer, ShannonEntropyComputer shannonEntropyComputer, string sequence)
        {
            double[] frequencies = frequencyComputer.computeFrequency(sequence);
            double entropy = shannonEntropyComputer.computeEntropy(frequencies);
            Console.WriteLine("The entropy of the sequence: {0}", entropy);
        }

        private static void computeJenShaDiv(FrequencyComputer frequencyComputer, JenShaDivComputer jenShaDivComputer, string sequence)
        {
            Console.WriteLine("Please give the position of sequence for Jensen-Shannon divergence compution:");
            int pos = Int32.Parse(Console.ReadLine());
            string sequencePrefix = sequence.Substring(0, pos);
            string sequencePostfix = sequence.Substring(pos);
            double[] frequenciesPrefix = frequencyComputer.computeFrequency(sequencePrefix);
            double[] frequenciesPostfix = frequencyComputer.computeFrequency(sequencePostfix);
            double weightPrefix = (double)sequencePrefix.Length / (double)sequence.Length;
            double weightPostfix = (double)sequencePostfix.Length / (double)sequence.Length;
            double divergence = jenShaDivComputer.computeDivergence(frequenciesPrefix, frequenciesPostfix, weightPrefix, weightPostfix);
            Console.WriteLine("The Jensen-Shannon divergence of the subsequences: {0}", divergence);
        }

        private static List<string> splitSequence(Splitter splitter, string sequence)
        {
            List<string> subsequences = splitter.split(sequence);
            if (subsequences.Count > 1)
            {
                Console.WriteLine("The splitted subsequences:");
                foreach (var subsequence in subsequences)
                {
                    Console.WriteLine(subsequence);
                }
            }
            else
            {
                Console.WriteLine("The sequence cannot be splitted because of significance threshold.");
            }
            return subsequences;
        }

        private static void saveSubsequences(List<string> subsequences)
        {
            Console.WriteLine("Please give the path with filename:");
            string file = Console.ReadLine();
            StreamWriter writer = new StreamWriter(file);
            foreach (var subsequence in subsequences)
            {
                writer.WriteLine(subsequence);
            }
            writer.Flush();
            writer.Dispose();
            writer = null;
        }
    }
}
