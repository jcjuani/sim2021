using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
namespace PseudoRandomNumbers.clases
{
    public static class Distributions
    {
        public static double InverseNormalDistribution(double probability)
        {
            Chart chart = new Chart();
            return chart.DataManipulator.Statistics.InverseNormalDistribution(probability);
        }
        public static double ChiSquare(int degreeOfFreedom, double significanceLevel)
        {
            int k = degreeOfFreedom - 1;
            double z = InverseNormalDistribution(1 - significanceLevel);
            double denominator = 9 * k;
            double quotient = 2 / denominator;
            double squaredRoot = Math.Sqrt(quotient);
            double cube = Math.Pow(1 - quotient + z * squaredRoot, 3);

            return k * cube;
        }
    }
}
