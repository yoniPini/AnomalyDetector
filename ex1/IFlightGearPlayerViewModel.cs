using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace ex1
{
    public interface IFlightGearPlayerViewModel : INotifyPropertyChanged
    {
        string VM_FG_Path { get; set; }
        string VM_XML_Path { get; set; }
        string VM_LearnCsv_Path { get; set; }
        string VM_TestCsv_Path { get; set; }
        bool VM_IsRunning { get; set; }
        bool VM_IsPaused { get; set; } // opposite of the above
        int VM_CurrentTimeStep { get; set; } // 950 for example
        int VM_MaxTimeStep { get; } // including the return value
        String VM_CurrentTimeInStr { get; } // "00:01:35 for example" 950 = 95 sec = 1 min + 35 sec
        double VM_SpeedTimes { get; set; } // 1.5 = "x1.5" speed

        //void Play();
        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;

    }
}
