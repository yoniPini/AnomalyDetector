using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ex1
{
    class FgAdapter
    {
        private FgTcp fgTcp;
        private string fgPath;
        private Process fgProcess;
        public bool IsFgRunning
        {
            get
            {
                return this.fgProcess != null;
            }
        }

        public delegate void action();

        public event action OnFGClosing;

        public FgAdapter(string fgDirPath)
        {
            this.fgPath = fgDirPath;
        }

        public bool Start(string protocolName, int dataPort = 5400, int telnetPort = 5402)
        {
            string FGargs = " --timeofday=noon" +
                                " --prop:/sim/menubar/visibility=false" +
                                " --prop:/sim/rendering/shaders/skydome=false" +
                                " --generic=socket,in,10,127.0.0.1," + dataPort + ",tcp," + protocolName +
                                " --fdm=null --prop:/sim/sound/voices/enabled=false" +
                                " --prop:/sim/view[1]/enabled=true --geometry=750x500" +
                                " --telnet=socket,in,10,127.0.0.1," + telnetPort + ",tcp" +
                                " --disable-ai-models --disable-sound";

            var psi = new ProcessStartInfo(this.fgPath + "fgfs.exe", FGargs);
            psi.WorkingDirectory = this.fgPath;
            this.fgProcess = Process.Start(psi);
            this.fgProcess.EnableRaisingEvents = true;
            this.fgProcess.Exited += delegate (object x, EventArgs e) { this.fgProcess = null; };
            this.fgProcess.Exited += delegate (object x, EventArgs e) { this.Close(); this.OnFGClosing?.Invoke(); };
            this.fgTcp = new FgTcp();

            return this.fgTcp?.Init(dataPort, telnetPort) ?? false;
        }

        public bool SendPlayback(string line)
        {
            return this.fgTcp?.SendPlayback(line) ?? false;
        }

        public void Close()
        {
            this.Close(true);
        }

        public void Close(bool alsoCloseFgWin)
        {
            this.fgTcp?.Close();
            this.fgTcp = null;
            if (alsoCloseFgWin)
            {
                try
                {
                    this.fgProcess?.Kill();
                    this.fgProcess?.Dispose();
                }
                catch { }
                this.fgProcess = null;
            }
            
        }
    }
}
