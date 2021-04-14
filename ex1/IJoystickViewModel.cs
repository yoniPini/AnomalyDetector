using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace ex1
{
    // interface of class that has access to get current features data, and give the needed data
    // to set up the joysitck(small circle in big circle, according to aileron and elevator)  ,
    // and its compoments(throttle and ruddle sliders)
    public interface IJoystickViewModel : INotifyPropertyChanged
    {
        float VM_throttle { get; }
        float VM_rudder { get; }

        // view -> view model
        double VM_Height_BigCircle { get; set; }
        double VM_Width_BigCircle { get; set; }
        Thickness VM_Margin_BigCircle { get; set; }
        Thickness VM_Margin_SmallCircle { get; }
        
        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;
    }
}
