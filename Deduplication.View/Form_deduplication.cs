using Deduplication.Controller.Algorithm;
using Deduplication.Model.DTO;
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
        ProgressForm _progressforms = new ProgressForm();

        public Form_deduplication()
        {
            InitializeComponent();
            InitAlgorithmCombobox();
        }

        private void InitAlgorithmCombobox()
        {
            var theInterface = typeof(IDeduplicationAlgorithm);
            var algorithmClasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()).Where(t => theInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            var comboboxDataSrc = algorithmClasses.Select(c => c.Name).ToList();
            comboBox_algorithm.DataSource = comboboxDataSrc;
            comboBox_algorithm.SelectedText = "BSW";
        }

        private void button_selectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog_srcPath.ShowDialog() == DialogResult.OK &&
                !string.IsNullOrWhiteSpace(folderBrowserDialog_srcPath.SelectedPath))
            {
                textBox_srcPath.Text = folderBrowserDialog_srcPath.SelectedPath;
            }
        }

        private void button_run_Click(object sender, EventArgs e)
        {
            _progressforms.Show();
            ClearProgress();
        }

        private void ClearProgress()
        {
            _progressforms.StartPosition = FormStartPosition.Manual;
            _progressforms.Location = new Point(this.Location.X + _progressforms.Width + 10, this.Location.Y);
            ProgressInfo tmp = new ProgressInfo()
            {
                Total = 1,
                Processed = 0,
                Message = ""
            };
            _progressforms.UpdateProgress(tmp, "files");
            _progressforms.UpdateProgress(tmp, "bytes");
            _progressforms.UpdateProgress(tmp, "chunks");
        }
    }
}
