using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.ApplicationServices;

namespace Echoes
{
    class App : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        public App()
        {
            // Make this a single-instance application
            this.IsSingleInstance = true;
            this.EnableVisualStyles = true;

            // There are some other things available in 
            // the VB application model, for
            // instance the shutdown style:
            this.ShutdownStyle =
              Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;

            // Add StartupNextInstance handler
            /*this.StartupNextInstance +=
              new StartupNextInstanceEventHandler(this.SIApp_StartupNextInstance);*/
        }
    }
}
