using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echoes
{
    public class ColumnInfo
    {
        public int id;
        public int width;
        public ColumnInfo(int identif, int wdt)
        {
            id = identif;
            width = wdt;
        }
    };
}
