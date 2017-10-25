using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Echoes;

namespace Echoes
{
    public partial class Equalizer : Form
    {
        const int TRACKBAR_MULT = 10;

        public Equalizer()
        {
            InitializeComponent();
            this.SetColors();
            eqEnableCheck.Checked = Program.mainWindow.eqEnabled;
            trackBar1.Value = BarVal(Program.mainWindow.eqGains[0]);
            trackBar2.Value = BarVal(Program.mainWindow.eqGains[1]);
            trackBar3.Value = BarVal(Program.mainWindow.eqGains[2]);
        }

        int BarVal(float val)
        {
            return (int)(val * TRACKBAR_MULT);
        }

        float TrueVal(int val)
        {
            return (float)val / TRACKBAR_MULT;
        }


        private void trackBar2_ValueChanged(object sender, MouseEventArgs e)
        {
            Program.mainWindow.eqGains[1] = TrueVal(trackBar2.Value);
            Program.mainWindow.ApplyEQ();
        }

        private void trackBar3_ValueChanged(object sender, MouseEventArgs e)
        {
            Program.mainWindow.eqGains[2] = TrueVal(trackBar3.Value);
            Program.mainWindow.ApplyEQ();
        }

        private void trackBar1_ValueChanged(object sender, MouseEventArgs e)
        {
            Program.mainWindow.eqGains[0] = TrueVal(trackBar1.Value);
            Program.mainWindow.ApplyEQ();
        }

        private void eqEnableCheck_CheckedChanged(object sender, EventArgs e)
        {
            Program.mainWindow.eqEnabled = eqEnableCheck.Checked;
            if (eqEnableCheck.Checked)
            {
                Program.mainWindow.DefineEQ();
                Program.mainWindow.ApplyEQ();
            }
            else
            {
                Program.mainWindow.RemoveEQ();
            }
        }
    }
}
