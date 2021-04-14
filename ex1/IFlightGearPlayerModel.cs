using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace ex1
{
    // interface of player on time steps playback.
    // which we can subsribed to PropertyChanged to get notice when CurrentTimeStep changed
    // or we can change it. ("skip" time in video)
    
    // this is the center of the program
    public interface IFlightGearPlayerModel : ITableSeriesNotify
    {
        double Const_OriginalHz { get; }    // how much timesteps was recorded in 1 sec
        string FG_Path { get; set; }        // fgfs.exe path
        string XML_Path { get; set; }       // xml record protocol
        string LearnCsv_Path { get; set; }
        string TestCsv_Path { get; set; }   // also known as "current" flight
        bool IsPowerOn { get; }
        bool IsRunning { get; set; }
        bool IsPaused { get; set; } // opposite of IsRunning
        int CurrentTimeStep { get; set; } // 950 for example
        int MaxTimeStep { get; } // The return value is "valid\exists" timestep
        String CurrentTimeInStr { get; } // "00:01:35 for example"
        double SpeedTimes { get; set; } // 1.5 = "x1.5" speed meaning pace of 1.5 * Const_OriginalHz timesteps per sec
        TableSeries Table { get; } // the data of the current/test flight
        void CloseFG(); // close the simulator(child process) 

        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;

        }
}
