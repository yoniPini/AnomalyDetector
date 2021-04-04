using System;
using System.IO;
using System.Threading;

using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var FG = new FgAdapter(@"C:\Program Files\FlightGear_2020.3.6\bin\");
            FG.Start();
            //fgfs.exe";
            int time = 100;
            string path = @"C:\Users\EhudV\Desktop\ap2_ex1\reg_flight.csv";
            using (FileStream fs = File.OpenRead(path)) {
                var sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    FG.SendPlayback(sr.ReadLine());
                    Thread.Sleep(time);
                }
            }

            Console.WriteLine("dsds\ndsds\ndsdsd\ndsdsd\ndsdsd\n");
            FG.Close(true);
            
        }
    }
}
