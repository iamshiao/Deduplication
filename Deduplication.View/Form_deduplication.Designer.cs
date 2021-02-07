namespace Deduplication.View
{
    partial class Form_deduplication
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_selectFolder = new System.Windows.Forms.Button();
            this.textBox_srcPath = new System.Windows.Forms.TextBox();
            this.comboBox_algorithm = new System.Windows.Forms.ComboBox();
            this.label_algorithm = new System.Windows.Forms.Label();
            this.button_run = new System.Windows.Forms.Button();
            this.dataGridView_storedFiles = new System.Windows.Forms.DataGridView();
            this.folderBrowserDialog_srcPath = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_storedFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // button_selectFolder
            // 
            this.button_selectFolder.Location = new System.Drawing.Point(12, 53);
            this.button_selectFolder.Name = "button_selectFolder";
            this.button_selectFolder.Size = new System.Drawing.Size(75, 32);
            this.button_selectFolder.TabIndex = 0;
            this.button_selectFolder.Text = "folder";
            this.button_selectFolder.UseVisualStyleBackColor = true;
            this.button_selectFolder.Click += new System.EventHandler(this.button_selectFolder_Click);
            // 
            // textBox_srcPath
            // 
            this.textBox_srcPath.Location = new System.Drawing.Point(93, 54);
            this.textBox_srcPath.Name = "textBox_srcPath";
            this.textBox_srcPath.Size = new System.Drawing.Size(695, 22);
            this.textBox_srcPath.TabIndex = 1;
            // 
            // comboBox_algorithm
            // 
            this.comboBox_algorithm.FormattingEnabled = true;
            this.comboBox_algorithm.Location = new System.Drawing.Point(93, 21);
            this.comboBox_algorithm.Name = "comboBox_algorithm";
            this.comboBox_algorithm.Size = new System.Drawing.Size(200, 24);
            this.comboBox_algorithm.TabIndex = 2;
            // 
            // label_algorithm
            // 
            this.label_algorithm.AutoSize = true;
            this.label_algorithm.Location = new System.Drawing.Point(21, 21);
            this.label_algorithm.Name = "label_algorithm";
            this.label_algorithm.Size = new System.Drawing.Size(67, 17);
            this.label_algorithm.TabIndex = 3;
            this.label_algorithm.Text = "Algorithm";
            // 
            // button_run
            // 
            this.button_run.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_run.Location = new System.Drawing.Point(713, 16);
            this.button_run.Name = "button_run";
            this.button_run.Size = new System.Drawing.Size(75, 32);
            this.button_run.TabIndex = 4;
            this.button_run.Text = "RUN";
            this.button_run.UseVisualStyleBackColor = true;
            this.button_run.Click += new System.EventHandler(this.button_run_Click);
            // 
            // dataGridView_storedFiles
            // 
            this.dataGridView_storedFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_storedFiles.Location = new System.Drawing.Point(12, 92);
            this.dataGridView_storedFiles.Name = "dataGridView_storedFiles";
            this.dataGridView_storedFiles.RowHeadersWidth = 51;
            this.dataGridView_storedFiles.RowTemplate.Height = 27;
            this.dataGridView_storedFiles.Size = new System.Drawing.Size(776, 300);
            this.dataGridView_storedFiles.TabIndex = 5;
            // 
            // Form_deduplication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 404);
            this.Controls.Add(this.dataGridView_storedFiles);
            this.Controls.Add(this.button_run);
            this.Controls.Add(this.label_algorithm);
            this.Controls.Add(this.comboBox_algorithm);
            this.Controls.Add(this.textBox_srcPath);
            this.Controls.Add(this.button_selectFolder);
            this.Name = "Form_deduplication";
            this.Text = "Deduplication";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_storedFiles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_selectFolder;
        private System.Windows.Forms.TextBox textBox_srcPath;
        private System.Windows.Forms.ComboBox comboBox_algorithm;
        private System.Windows.Forms.Label label_algorithm;
        private System.Windows.Forms.Button button_run;
        private System.Windows.Forms.DataGridView dataGridView_storedFiles;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog_srcPath;
    }
}

