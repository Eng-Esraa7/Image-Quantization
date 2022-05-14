using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            if (K_value.Text != "")
            {
                //first get distinct color
                process.DistinctColor();
                dist_txt.Text = process.DistinctColorList.Count.ToString();

                //second Mst
                double mst_sum = process.Generate_MST();
                mst_sum = Math.Round(mst_sum, 2);
                mst_txt.Text = mst_sum.ToString();
                HashSet<HashSet<int>> C = process.clusters;
                process.Cluster(int.Parse(K_value.Text));
                MessageBox.Show(C.Count.ToString());
                

                double sigma = double.Parse(txtGaussSigma.Text);
                int maskSize = (int)nudMaskSize.Value;
                ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox2);



                //finally clear lists
                process.DistinctColorList.Clear();
                process.MST.Clear();
                C.Clear();
            }
            else
                MessageBox.Show("should enter k");
        }
    }
}