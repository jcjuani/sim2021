using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PseudoRandomNumbers.clases.generadores;
using PseudoRandomNumbers.clases;

namespace PseudoRandomNumbers
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            cmb_generador.SelectedIndex = 2;
            cmbIntervalos.SelectedIndex = 0;
        }

        private void btn_iniciar_Click(object sender, EventArgs e)
        {
            ClearData();
            RandomGenerator rg = CreateRandomGenerator();

            if (msk_n.Text == "") msk_n.Text = "10";
            int n = Int32.Parse(msk_n.Text);
            float[] numbers = CreateRandomArray(n, rg);
            int numberOfIntervals = Int32.Parse(cmbIntervalos.SelectedItem.ToString());
            ProcessData(numberOfIntervals, numbers);
        }

        private LCG CreateLCG()
        {
            int seed = Int32.Parse(msk_seed.Text);
            int k = Int32.Parse(msk_k.Text);
            int g = Int32.Parse(msk_g.Text);
            int c = Int32.Parse(msk_c.Text);

            return new LCG(seed, k, g, c);
        }

        private MCG CreateMCG()
        {
            int seed = Int32.Parse(msk_seed.Text);
            int k = Int32.Parse(msk_k.Text);
            int g = Int32.Parse(msk_g.Text);

            return new MCG(seed, k, g);
        }

        private RandomGenerator CreateStandardGenerator()
        {
            if (msk_seed.Text == "") msk_seed.Text = "0";
            int seed = Int32.Parse(msk_seed.Text);

            return new StandardGenerator(seed);
        }

        private RandomGenerator CreateRandomGenerator()
        {
            if(cmb_generador.SelectedIndex == 0)
            {
                return CreateLCG();
            }
            if(cmb_generador.SelectedIndex == 1)
            {
                return CreateMCG();
            }

            return CreateStandardGenerator();
        }

        private float[] CreateRandomArray(int n, RandomGenerator rg)
        {
            float[] numbers = new float[n];

            for(int i = 0; i < n; i++)
            {
                numbers[i] = rg.GetNextRandomNumber();
            }

            return numbers;
        }

        private string ListToString(IList<float> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for(int i = 0; i < list.Count; i++)
            {
                sb.Append(Math.Round(list[i], 4));
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        private void ClearData()
        {
            txt_output.Clear();
            txtChiCuadrado.Clear();
            dgvTabla.Rows.Clear();
        }
        private void ProcessData(int numberOfIntervals, IList<float> numbers)
        {
            GenerateTable(numberOfIntervals, numbers);
            txt_output.AppendText(ListToString(numbers));
            txtChiCuadrado.Text = Distributions.ChiSquare(numberOfIntervals, 0.05).ToString();
        }

        private void GenerateTable(int numberOfIntervals, IList<float> numbers)
        {
            AddRows(numberOfIntervals, numbers);
            AddObservations(numbers, numberOfIntervals);
            AddStatistics();
            dgvTabla.Visible = true;
        }
        private void AddStatistics() 
        {
            double accumulatedStatistic = 0;
            foreach(DataGridViewRow row in dgvTabla.Rows)
            {
                double expectedFrecuency = Double.Parse(row.Cells[3].Value.ToString());
                Int32 observedFrecuency = Int32.Parse(row.Cells[2].Value.ToString());
                double statistic = Math.Pow(expectedFrecuency - observedFrecuency, 2)/expectedFrecuency;
                accumulatedStatistic += statistic;

                row.Cells[4].Value = statistic;
                row.Cells[5].Value = accumulatedStatistic;
            }
        }
        private void AddObservations(IList<float> numbers, int numberOfIntervals)
        {
            foreach(float number in numbers)
            {
                int index = FindIndex(number);
                int observedValue = Int32.Parse(dgvTabla.Rows[index].Cells[2].Value.ToString()) + 1;
                dgvTabla.Rows[index].Cells[2].Value = observedValue;
            }
        }
        private int FindIndex(float number)
        {
            foreach(DataGridViewRow row in dgvTabla.Rows)
            {
                DataGridViewCellCollection cells = row.Cells;
                double lowerLimit = Double.Parse(cells[0].Value.ToString());
                double  upperLimit = Double.Parse(cells[1].Value.ToString());
                if(number >= lowerLimit && number < upperLimit)
                {
                    return dgvTabla.Rows.IndexOf(row);
                }
            }
            return -1;
        }
        private void AddRows(int quantity, IList<float> numbers)
        {
            int numberOfIntervals = quantity;
            double classWidth = (float) 1 / numberOfIntervals;
            double expectedFrecuency = (double)numbers.Count / numberOfIntervals;
            for (int i = 0; i < numberOfIntervals; i++)
            {
                double lowerLimit = Truncate(classWidth * i, 2);
                double upperLimit = Truncate(classWidth * (i + 1), 2);
                dgvTabla.Rows.Add(lowerLimit, upperLimit, 0, expectedFrecuency , 0, 0);
            }
        }

        private double Truncate(double number, byte decimals)
        {
            double limit = Math.Pow(10, decimals);
            double temp = Math.Truncate(number * limit);
            return temp / limit;
        }
    }
}
