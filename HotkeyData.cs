using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echoes
{
    public class HotkeyData
    {
        public Hotkey hotkey;
        public Keys key;
        //public int mod;
        public bool alt, shift, ctrl;
        public bool enabled;
        public bool registered = false;
        public HotkeyData(Hotkey hotkey, Keys key, bool ctrl, bool alt, bool shift, bool enabled)
        {
            this.hotkey = hotkey;
            this.key = key;
            this.alt = alt;
            this.shift = shift;
            this.ctrl = ctrl;
            this.enabled = enabled;
        }

        public string ModToString()
        {
            if (!(alt || shift || ctrl)) return "None";
            List<string> btns = new List<string>();
            string ret = "";
            if (ctrl) btns.Add("Ctrl");
            if (alt) btns.Add("Alt");
            if (shift) btns.Add("Shift");
            for (int i = 0; i < btns.Count; i++)
            {
                ret += btns[i];
                if (i < btns.Count - 1)
                {
                    ret += " + ";
                }
            }
            return ret;
        }
    }
}
