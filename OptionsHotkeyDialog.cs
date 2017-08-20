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
        public OptionsHotkeyDialog(Hotkey fnc, int mod, int key, bool enabled)
        {
            InitializeComponent();
            comboBox1.DataSource = Enum.GetValues(typeof(Hotkey));
            comboBox2.DataSource = Enum.GetValues(typeof(Modifier));
            comboBox3.DataSource = Enum.GetValues(typeof(Modifier));
            for (int i = Program.mainWindow.modValues.Length-1; i >= 0 ; i--)
            {
                if (mod >= Program.mainWindow.modValues[i]) { comboBox2.SelectedIndex = i; mod -= Program.mainWindow.modValues[i]; break; }
            }
            for (int i = Program.mainWindow.modValues.Length-1; i >= 0; i--)
            {
                if (mod >= Program.mainWindow.modValues[i]) { comboBox3.SelectedIndex = i; break; }
            }
            comboBox1.SelectedIndex = (int)fnc;
            textBox1.Text = ((Keys)key).ToString();
            enteredKey = (Keys)key;
            checkBox1.Checked = enabled;
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
            return new HotkeyData((Hotkey)comboBox1.SelectedValue, enteredKey, Program.mainWindow.modValues[(int)comboBox2.SelectedValue] + Program.mainWindow.modValues[(int)comboBox3.SelectedValue], checkBox1.Checked);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
