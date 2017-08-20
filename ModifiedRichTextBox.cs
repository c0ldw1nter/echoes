using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ModifiedControls
{
    public class ModifiedRichTextBox : RichTextBox
    {
        public ModifiedRichTextBox()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.BorderStyle = BorderStyle.None;
        }

        private const int WM_PAINT = 15;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PAINT)
            {
                this.Invalidate();
                base.WndProc(ref m);
                using (Graphics g = Graphics.FromHwnd(this.Handle))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
