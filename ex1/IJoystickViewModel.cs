using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace ex1
{
    public interface IJoystickViewModel : INotifyPropertyChanged
    {
        float VM_throttle { get; }
        float VM_rudder { get; }
        //float VM_rudder { get; }
        //float VM_throttle { get; }

        // view -> view model
        double VM_Height_BigCircle { get; set; }
        double VM_Width_BigCircle { get; set; }
        Thickness VM_Margin_BigCircle { get; set; }
        Thickness VM_Margin_SmallCircle { get; }
        //int VM_x_SmallCircle { get; set; }
        //int VM_y_SmallCircle { get; set; }
        //maybe more adjustible propties for the joysick

        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;
    }
}
