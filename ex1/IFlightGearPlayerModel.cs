using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace ex1
{
    public interface IFlightGearPlayerModel : INotifyPropertyChanged
    {
        string FG_Path { get; set; }
        string XML_Path { get; set; }
        string LearnCsv_Path { get; set; }
        string TestCsv_Path { get; set; }
        bool IsRunning { get; set; }
        bool IsPaused { get; set; } // opposite of the above
        int CurrentTimeStep { get; set; } // 950 for example
        int MaxTimeStep { get; } // including the return value
        String CurrentTimeInStr { get; } // "00:01:35 for example"
        double SpeedTimes { get; set; } // 1.5 = "x1.5" speed
        TableSeries Table { get; }
        void CloseFG();
            //void Play();
            // INotifyPropertyChanged:
            // event PropertyChangedEventHandler PropertyChanged;

        }
}
