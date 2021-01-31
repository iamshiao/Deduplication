using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deduplication.View
{
    public partial class Form_deduplication : Form
    {
        public Form_deduplication()
        {
            InitializeComponent();
        }

        private void button_selectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog_srcPath.ShowDialog() == DialogResult.OK &&
                !string.IsNullOrWhiteSpace(folderBrowserDialog_srcPath.SelectedPath))
            {
                textBox_srcPath.Text = folderBrowserDialog_srcPath.SelectedPath;
            }
        }
    }
}
