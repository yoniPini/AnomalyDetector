using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ex1
{
    public interface IJoystickModel : INotifyPropertyChanged
    {
        float aileron { get; }
        float elevator { get; }
        float rudder { get; }
        float throttle { get; }
        
        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;

    }
}
