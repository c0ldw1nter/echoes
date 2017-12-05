using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Echoes
{
    public partial class Converter : Form
    {
        public List<string> files;
        public string outputPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "converted");

        public Converter()
        {
            InitializeComponent();
        }

        public Converter(List<string> files)
        {
            InitializeComponent();
            this.files = files;
        }

        void QueueConversion()
        {
            if (files.Count <= 0) return;

 
        }

        private void convertWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void convertWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void convertWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void outputPathButton_Click(object sender, EventArgs e)
        {

        }

        private void convertButton_Click(object sender, EventArgs e)
        {

        }
    }
}
