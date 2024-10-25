using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FT_Parameter_Files
{
    public partial class Form1 : Form
    {
        string pathHOA, pathCSV, folder;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                pathHOA = file.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // read file to string
            string hoaText = System.IO.File.ReadAllText(pathHOA);
            // read csv to array
            StreamReader sr = new StreamReader(pathCSV);
            string line;
            string[] row = new string[2];
            while ((line = sr.ReadLine()) != null)
                {
                    row = line.Split(',');

                // for every csv line, make file
                string text = hoaText.Replace("YC_1XX10", row[0]);
                text = text.Replace("TAG_HOLDER", row[0].Replace("XX", "  "));
                text = text.Replace("DESC_HOLDER", row[1].Replace(" ", " "));
                File.WriteAllText(folder + "\\" + row[0] + ".par", text, Encoding.Unicode);
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog file = new FolderBrowserDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                folder = file.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                pathCSV = file.FileName;
            }
        }
    }
}
