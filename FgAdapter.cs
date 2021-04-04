using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApp1
{
    class FgAdapter
    {
        private FgTcp fgTcp;
        private string fgPath;
        private Process fgProcess;
        public FgAdapter(string fgPath)
        {
            this.fgPath = fgPath;
        }
        public bool Start()
        {
            string FGargs = " --timeofday=noon" +
                                " --prop:/sim/menubar/visibility=false" +
                                " --prop:/sim/rendering/shaders/skydome=false" +
                                " --generic=socket,in,10,127.0.0.1,5400,tcp,playback_small" +
                                " --fdm=null --prop:/sim/sound/voices/enabled=false" +
                                " --prop:/sim/view[1]/enabled=true --geometry=750x500" +
                                " --telnet=socket,in,10,127.0.0.1,5402,tcp --disable-ai-models";

            var psi = new ProcessStartInfo(this.fgPath + "fgfs.exe", FGargs);
            psi.WorkingDirectory = this.fgPath;
            this.fgProcess = Process.Start(psi);
            this.fgProcess.Exited += delegate (object x, EventArgs e) { this.fgProcess = null; };
            this.fgTcp = new FgTcp();

            return this.fgTcp.Init(5400, 5402);
        }
        
        public bool SendPlayback(string line)
        {
            return this.fgTcp.SendPlayback(line);
        }

        public void Close(bool alsoCloseFgWin)
        {
            this.fgTcp.Close();
            this.fgTcp = null;
            if (alsoCloseFgWin)
            {
                this.fgProcess?.Kill();
                this.fgProcess?.Dispose();
                this.fgProcess = null;
            }

        }
        public bool IsFgRunning { get
            {
                return this.fgProcess != null;
            } }

        //send close onexit
    }
}
