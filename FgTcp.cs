using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace ConsoleApp1
{
    class FgTcp
    {
        private Socket telnet;
        private Socket playback;

        private bool IsFGReady()
        {   
            telnet.Send(ASCIIEncoding.ASCII.GetBytes("get /sim/fdm-initialized\r\n"));
            byte[] buffer = new byte[100];
            telnet.Receive(buffer);
            string answer = new String(ASCIIEncoding.ASCII.GetChars(buffer));
            return answer.Split(' ')[2] == "'true'";
        }
        public bool Init(int playbackPort, int telnetPort)
        {
            int tries = 10;
            bool error = true;
            while (tries > 0)
            {
                tries--;
                try
                {
                    this.telnet = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    this.telnet.Connect("localhost", telnetPort);

                    this.playback = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    this.playback.Connect("localhost", playbackPort);
                    tries = 0;
                    error = false;
                }
                catch 
                {
                    Thread.Sleep(1000);
                }
            }

            if (error)
                return false;

            try
            {
                while (!IsFGReady())
                    Thread.Sleep(1000);

                Thread.Sleep(1000);

                telnet.Send(ASCIIEncoding.ASCII.GetBytes("set /sim/view[1]/enabled true\r\n"));
                telnet.Send(ASCIIEncoding.ASCII.GetBytes("set /sim/current-view/view-number 1\r\n"));
                telnet.Send(ASCIIEncoding.ASCII.GetBytes("set /sim/current-view/z-offset-m -14\r\n"));
                telnet.Send(ASCIIEncoding.ASCII.GetBytes("set /sim/current-view/pitch-offset-deg 3\r\n"));
                telnet.Close();
                telnet.Dispose();
            } catch {
                return false;
            }
            return true;
        }

        public bool SendPlayback(string line)
        {
            try
            {
                this.playback.Send(ASCIIEncoding.ASCII.GetBytes(line+"\r\n"));
                return true;
            } catch
            {
                return false;
            }
        }

        public void Close()
        {
            this.playback.Close();
            this.playback.Dispose();
        }
    }
}
