using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public static RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            //check if enter k or not
            if (K_value.Text != "")
            {
                //start timer
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                //first get distinct color
                process.DistinctColor();
                //print num of distinct color in textbox
                dist_txt.Text = process.DistinctColorList.Count.ToString();

                //second Mst
                double mst_sum = process.Generate_MST();
                //round mst sum
                mst_sum = Math.Round(mst_sum, 2);
                //print mst sum in textbox
                mst_txt.Text = mst_sum.ToString();

                //third clustring
                HashSet<HashSet<int>> Clusters = process.clusters;
                process.Cluster(int.Parse(K_value.Text));
                int num_of_clusters = (Clusters.Count)-1;
                //print num of clusters
                textBox1.Text = num_of_clusters.ToString();

                //fourth Quantization
                //get avg each cluster
                process.ImageDictionary();
                process.Quantization();

                double sigma = double.Parse(txtGaussSigma.Text);
                int maskSize = (int)nudMaskSize.Value;
                ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox2);

                //stop watch
                stopwatch.Stop();
                TimeSpan time = stopwatch.Elapsed;
                timer.Text = time.ToString();

                //finally clear lists
                process.DistinctColorList.Clear();
                process.MST.Clear();
                Clusters.Clear();
                process.resultImageDictionary.Clear();
            }
            else
                MessageBox.Show("should enter k");
        }
    }
}