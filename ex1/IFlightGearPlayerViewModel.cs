using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace ex1
{
    // interface which is between the view and the IFlightGearPlayerModel,
    // to give data to the view about current state of the playing(timestep, for example)
    // and to give the IFlightGearPlayerModel commands / data such as VM_FG_Path (bound to textbox in view)
    public interface IFlightGearPlayerViewModel : INotifyPropertyChanged
    {
        double Const_OriginalHz { get; } // how many timestep there is in one sec
        string VM_FG_Path { get; set; }
        string VM_XML_Path { get; set; }
        string VM_LearnCsv_Path { get; set; }
        string VM_TestCsv_Path { get; set; }
        bool VM_IsPowerOn { get; } // is there established connection with the fgfs.exe (fg simulator)
        bool VM_IsPowerOff { get; } // the opposite
        bool VM_IsRunning { get; set; }
        bool VM_IsPaused { get; set; } // opposite of the above
        int VM_CurrentTimeStep { get; set; } // 950 for example
        int VM_MaxTimeStep { get; } // including the return value
        String VM_CurrentTimeInStr { get; } // "00:01:35 for example" 950 = 95 sec = 1 min + 35 sec
        double VM_SpeedTimes { get; set; } // 1.5 = "x1.5" speed
        void CloseFG();

        //void Play();
        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;

    }
}
