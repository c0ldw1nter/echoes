using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Echoes;

namespace ModifiedControls
{
    public class EqualizerTrackbar : Control
    {
        float Value;
        public float TheValue
        {
            get { return Value; }
            set
            {
                Value = value;
                Refresh();
            }
        }
        public float max;
        public float min;
        private bool dragging = false;

        public EqualizerTrackbar()
        {
            max = 150;
            min = -150;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawRectangle(Pens.Blue, 0, 0, Height, Width);
            //g.FillRectangle(Pens.Blue,)
        }

        float GetValueFromCoordinate(int y)
        {
            if (y > Height) return 0;
            if (y < 0) return max;

            float range = max - min;
            float pixelValueRatio = (float)range / (float)Height;

            float value = y * pixelValueRatio;
            return max - value;
        }

        int GetCoordinateFromValue(float v)
        {
            if (v < min) return (int)min;
            if (v > max) return (int)max;

            float range = max - min;
            float pixelValueRatio = (float)range / (float)Height;

            float val = max - v;
            val = range - val;

            return 0;
            //wtf
        }

        void UpdateToMouse(MouseEventArgs e)
        {
            TheValue = GetValueFromCoordinate(e.Y);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            dragging = true;
            UpdateToMouse(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragging = false;
            UpdateToMouse(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateToMouse(e);
        }
    }
}
