using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace ex1
{
    // class to send init setting via telnet,
    // and playbacks [features values in current timestep]
    // to an open fg simulator, via tcp,Ipv4, localhost
    class FgTcp
    {
        private Socket telnet;
        private Socket playback;

        // check if the graphics of the fg is ready
        private bool IsFGReady()
        {
            // Dont put it in try and catch [see the use in Init method]
            telnet?.Send(ASCIIEncoding.ASCII.GetBytes("get /sim/fdm-initialized\r\n"));
            byte[] buffer = new byte[100];
            telnet?.Receive(buffer);
            string answer = new String(ASCIIEncoding.ASCII.GetChars(buffer));
            return answer.Split(' ')[2] == "'true'";
        }

        // send const setting in telnet, in order to make the simulator be seen well,
        // return whether it succeeded [code waiting]
        public bool Init(int playbackPort, int telnetPort)
        {
            // try to connect to playback and to telnet, give it 10 tries
            int tries = 10;
            bool error = true;
            while (tries > 0)
            {
                tries--;
                try
                {
                    this.telnet = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    this.telnet?.Connect("localhost", telnetPort);

                    this.playback = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    this.playback?.Connect("localhost", playbackPort);
                    tries = 0;
                    error = false;
                }
                catch
                {   
                    // after each failure wait 1 second to give the simulator another chance to start up
                    Thread.Sleep(1000);
                }
            }

            if (error)
                return false;

            try
            {
                // wait until the graphic are ready
                // if simulator is shut down then exception is thrown
                while (!IsFGReady())
                    Thread.Sleep(1000);

                // wait another second
                Thread.Sleep(1000);

                // some set-up
                telnet?.Send(ASCIIEncoding.ASCII.GetBytes("set /sim/view[1]/enabled true\r\n"));
                telnet?.Send(ASCIIEncoding.ASCII.GetBytes("set /sim/current-view/view-number 1\r\n"));
                telnet?.Send(ASCIIEncoding.ASCII.GetBytes("set /sim/current-view/z-offset-m -14\r\n"));
                telnet?.Send(ASCIIEncoding.ASCII.GetBytes("set /sim/current-view/pitch-offset-deg 3\r\n"));
                telnet?.Close();
                telnet?.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        // send playback like "0,0.4,9,8.9" to the simulator playback socket
        public bool SendPlayback(string line)
        {
            try
            {
                this.playback?.Send(ASCIIEncoding.ASCII.GetBytes(line + "\r\n"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Close()
        {
            this.playback?.Close();
            this.playback?.Dispose();
        }
    }
}
