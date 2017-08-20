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
        public int mod;
        public bool enabled;
        public bool registered = false;
        public HotkeyData(Hotkey hotkey, Keys key, int mod, bool enabled)
        {
            this.hotkey = hotkey;
            this.key = key;
            this.mod = mod;
            this.enabled = enabled;
        }
    }
}
