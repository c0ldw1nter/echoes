using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ModifiedControls
{
    public class ModifiedButton : Button
    {

        public ModifiedButton()
            : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            base.FlatAppearance.BorderColor = BackColor;
            base.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        }
    }
}
