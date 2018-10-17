namespace Echoes
{
    partial class Exporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Exporter));
            this.infoLabel = new System.Windows.Forms.Label();
            this.exporterWorker = new System.ComponentModel.BackgroundWorker();
            this.log = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(12, 9);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(35, 13);
            this.infoLabel.TabIndex = 1;
            this.infoLabel.Text = "label1";
            // 
            // exporterWorker
            // 
            this.exporterWorker.WorkerReportsProgress = true;
            this.exporterWorker.WorkerSupportsCancellation = true;
            this.exporterWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.exporterWorker_DoWork);
            this.exporterWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.exporterWorker_ProgressChanged);
            this.exporterWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.exporterWorker_RunWorkerCompleted);
            // 
            // log
            // 
            this.log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log.Location = new System.Drawing.Point(12, 25);
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.Size = new System.Drawing.Size(560, 225);
            this.log.TabIndex = 2;
            this.log.Text = "";
            // 
            // Exporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 262);
            this.Controls.Add(this.log);
            this.Controls.Add(this.infoLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Exporter";
            this.Text = "Exporter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label infoLabel;
        private System.ComponentModel.BackgroundWorker exporterWorker;
        private System.Windows.Forms.RichTextBox log;
    }
}