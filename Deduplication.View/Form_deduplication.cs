using Deduplication.Controller;
using Deduplication.Controller.Algorithm;
using Deduplication.Controller.Extensions;
using Deduplication.Model.DAL;
using Deduplication.Model.DTO;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deduplication.View
{
    public partial class Form_deduplication : Form
    {
        ProgressForm _progressforms = new ProgressForm();
        IStorage _storage;

        public Form_deduplication()
        {
            InitializeComponent();
            InitAlgorithmCombobox();
            InitReassemblyBtnColumnToGrid();
            InitStorageCombobox();
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

        private void InitStorageCombobox()
        {
            var theInterface = typeof(IStorage);
            var storageClasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()).Where(t => theInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            var comboboxDataSrc = storageClasses.Select(c => c.Name).ToList();
            comboBox_storage.DataSource = comboboxDataSrc;
            comboBox_storage.SelectedText = "MemoryStorage";
        }

        private void InitReassemblyBtnColumnToGrid()
        {
            DataGridViewButtonColumn reassemblyBtnCol = new DataGridViewButtonColumn();
            reassemblyBtnCol.Name = "";
            reassemblyBtnCol.Text = "Reassembly";
            reassemblyBtnCol.UseColumnTextForButtonValue = true;
            int columnIndex = 0;
            if (dataGridView_storedFiles.Columns["reassemblyBtnCol"] == null)
            {
                dataGridView_storedFiles.Columns.Insert(columnIndex, reassemblyBtnCol);
            }
        }

        private void button_selectFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog_srcPath.SelectedPath = Path.GetFullPath(Properties.Settings.Default.DefaultPath);
            if (folderBrowserDialog_srcPath.ShowDialog() == DialogResult.OK &&
                !string.IsNullOrWhiteSpace(folderBrowserDialog_srcPath.SelectedPath))
            {
                textBox_srcPath.Text = folderBrowserDialog_srcPath.SelectedPath;
            }
            Properties.Settings.Default.DefaultPath = folderBrowserDialog_srcPath.SelectedPath;
            Properties.Settings.Default.Save();
        }

        private async void button_run_Click(object sender, EventArgs e)
        {
            var filePaths = Directory.GetFiles(textBox_srcPath.Text, "*.*", SearchOption.TopDirectoryOnly);
            var fileInfos = filePaths.Select(path => new FileInfo(path));

            _progressforms.Show();
            ClearProgress();

            var algSelected = comboBox_algorithm.Text;
            if (comboBox_storage.Text == "MemoryStorage")
                _storage = new MemoryStorage();
            else if (comboBox_storage.Text == "LocalStorage")
            {
                _storage = new LocalStorage(algSelected, _progressforms.UpdateProgress);
            }

            DeduplicateController deduCtrl = new DeduplicateController(algSelected, _storage, _progressforms.UpdateProgress);
            await Task.Run(() => { deduCtrl.ImportFiles(fileInfos); });
            var storedFiles = _storage.GetAllFileViewModels().ToList();
            var fvmGridSrc = storedFiles.Select(fvm => new
            {
                Name = fvm.Name,
                Size = GeneralExtension.SizeSuffix(fvm.Size),
                ChunkCount = fvm.Chunks.Count(),
                ProcessTime = $"{fvm.ProcessTime:hh\\:mm\\:ss\\:ms}"
            }).ToList();
            dataGridView_storedFiles.DataSource = fvmGridSrc;
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
            _progressforms.UpdateProgress(tmp, "reassembly");
        }

        private void dataGridView_storedFiles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView_storedFiles.Columns[""].Index && (sender as DataGridView).DataSource != null)
            {
                var fileName = (string)dataGridView_storedFiles.Rows[e.RowIndex].Cells["Name"].Value;

                var fvm = _storage.GetAllFileViewModels().FirstOrDefault(x => x.Name == fileName);
                saveFileDialog_reassemblyTo.InitialDirectory = folderBrowserDialog_srcPath.SelectedPath;
                saveFileDialog_reassemblyTo.FileName = fileName;
                if (saveFileDialog_reassemblyTo.ShowDialog() == DialogResult.OK &&
                    !string.IsNullOrWhiteSpace(saveFileDialog_reassemblyTo.FileName))
                {
                    // Show progress form for reassembly
                    _progressforms.Show();
                    
                    // Clear previous progress
                    var clearProgress = new ProgressInfo(1, 0, "");
                    _progressforms.UpdateProgress(clearProgress, "reassembly");
                    
                    // Start reassembly with progress reporting
                    Task.Run(() =>
                    {
                        _storage.Reassembly(fvm, saveFileDialog_reassemblyTo.FileName, _progressforms.UpdateProgress);
                        
                        // Hide progress form when done
                        this.Invoke(new Action(() => _progressforms.Hide()));
                    });
                }
            }
        }
    }
}
