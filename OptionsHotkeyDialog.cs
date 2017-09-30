using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echoes
{
    public partial class OptionsHotkeyDialog : Form
    {
        Keys enteredKey;
        public HotkeyData returnedData;
        public OptionsHotkeyDialog(Hotkey fnc, bool ctrl, bool alt, bool shift, int key, bool enabled)
        {
            InitializeComponent();
            funcCombo.DataSource = Enum.GetValues(typeof(Hotkey));
            funcCombo.SelectedIndex = (int)fnc;
            ctrlCheck.Checked = ctrl;
            altCheck.Checked = alt;
            shiftCheck.Checked = shift;
            textBox1.Text = ((Keys)key).ToString();
            enteredKey = (Keys)key;
            enabledCheck.Checked = enabled;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            textBox1.Text = e.KeyCode.ToString();
            enteredKey = e.KeyCode;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            returnedData = ReturnData();
            this.DialogResult = DialogResult.OK;
        }

        HotkeyData ReturnData()
        {
            return new HotkeyData((Hotkey)funcCombo.SelectedValue, enteredKey, ctrlCheck.Checked, altCheck.Checked, shiftCheck.Checked, enabledCheck.Checked);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
