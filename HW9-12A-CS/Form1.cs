using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MyHomework
{
    public partial class Form1 : Form
    {
        private int m;          // n. of paths
        private int n;          // number of points for each path
        private double sigma;   // distribution variance
        private int t;          // point at time T
        private int c;          // nb of clusters for histograms

        private Distribution RN;

        private ggPictureBox ggPictureBox1;

        public Form1()
        {
            InitializeComponent();

            ggPictureBox1 = new ggPictureBox(MainPanel);
            ggPictureBox1.BackColor = Color.White;
            ggPictureBox1.Top = MainPanel.Height / 10; ;
            ggPictureBox1.Left = MainPanel.Left + MainPanel.Width / 10;
            ggPictureBox1.Height = MainPanel.Height / 10 * 8;
            ggPictureBox1.Width = MainPanel.Width / 10 * 8;
            ggPictureBox1.BorderStyle = BorderStyle.FixedSingle;
            MainPanel.Controls.Add(ggPictureBox1);

            tbTPoint.Minimum = 1;
            tbTPoint.Maximum = (int)NbPoints.Value;
            tbTPoint.Value = (int)Math.Ceiling((double)NbPoints.Value / 2);

            NbPoints.Value = 10;
            NbClusters.Value = 10;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetVariables();
        }

        // Events
        //--------------------------------------------------------

        private void btnRecalc_Click(object sender, EventArgs e)
        {
            SetVariables();

            tbTPoint.Minimum = 1;
            tbTPoint.Maximum = n;

            CreateStatEngineInstance();
            DrawChart();
        }

        private void NbPoints_ValueChanged(object sender, EventArgs e)
        {
            SetVariables();

            tbTPoint.Maximum = n;
            t = tbTPoint.Value = (int)Math.Ceiling((double)n / 2);
            //tbTPoint.TickFrequency = tbTPoint.Maximum / 10;

            CreateStatEngineInstance();
            DrawChart();
        }

        private void variance_ValueChanged(object sender, EventArgs e)
        {
            SetVariables();
            CreateStatEngineInstance();
            DrawChart();
        }

        private void NbPath_ValueChanged(object sender, EventArgs e)
        {
            SetVariables();
            CreateStatEngineInstance();
            DrawChart();
        }

        private void cmbDistribution_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetVariables();
            CreateStatEngineInstance();
            DrawChart();
        }

        private void tbTPoint_ValueChanged(object sender, EventArgs e)
        {
            t = tbTPoint.Value;
            DrawChart();
        }

        private void btnClusters_Click(object sender, EventArgs e)
        {
            SetVariables();
            DrawChart();
        }

        private void NbClusters_ValueChanged(object sender, EventArgs e)
        {
            SetVariables();
            DrawChart();
        }

        // Methods
        //--------------------------------------------------------

        private void SetVariables()
        {
            n = (int)NbPoints.Value;
            m = (int)NbPath.Value;
            sigma = (double)variance.Value;
            t = tbTPoint.Value;
            c = (int)NbClusters.Value;
        }

        private void CreateStatEngineInstance()
        {
            RN = new Distribution(n, m, sigma);
            RN.Paths = RN.GenerateDistribution();
        }

        private void DrawChart()
        {
            if (RN != null)
            {
                ChartManager CM = new ChartManager(RN, ggPictureBox1, t);
                CM.DrawChart(c);
            }
        }
    }
}
