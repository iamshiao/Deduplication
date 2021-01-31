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
    public partial class ProgressForm : Form
    {
        private class ProgressBarContollers
        {
            public ProgressBarContollers(Label proportion, Label message, ProgressBar progress, Label percentage)
            {
                Proportion = proportion;
                Message = message;
                Progress = progress;
                Percentage = percentage;
            }
            public Label Proportion;
            public Label Message;
            public ProgressBar Progress;
            public Label Percentage;
        }
        Dictionary<string, ProgressBarContollers> _progressBars;
        public ProgressForm()
        {
            InitializeComponent();
            _progressBars = new Dictionary<string, ProgressBarContollers>()
            {
                {"files", new ProgressBarContollers(label_proportion_files, label_msg_files, progressBar_files, label_percentage_files) },
                {"bytes", new ProgressBarContollers(label_proportion_bytes, label_msg_bytes, progressBar_bytes, label_percentage_bytes) },
                {"chunks", new ProgressBarContollers(label_proportion_chunks, label_msg_chunks, progressBar_chunks, label_percentage_chunks) }
            };
        }

        public void UpdateProgress(ProgressInfo pi, string name)
        {
            if (!_progressBars.ContainsKey(name))
            {
                throw new Exception();
            }
            this.Invoke(new Action(() =>
            {
                _progressBars[name].Proportion.Text = $"{pi.Processed}/{pi.Total}";
                _progressBars[name].Message.Text = pi.Message;
                _progressBars[name].Progress.Value = (int)Math.Round((double)(100 * pi.Processed) / pi.Total);
                _progressBars[name].Percentage.Text = $"{_progressBars[name].Progress.Value} %";
            }));
        }

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
