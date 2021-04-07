using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ex1
{
    public class JoystickModel : IJoystickModel
    {
        public float aileron
        { get{
                    float x = fgModel?.Table?.getCell(fgModel.CurrentTimeStep, "aileron") ?? 0;
                    return x;
            }}
        public float elevator { get{
                    float x = fgModel?.Table?.getCell(fgModel.CurrentTimeStep, "elevator") ?? 0;
                    return x;
            }}
        public float rudder { get{
                    float x =  fgModel?.Table?.getCell(fgModel.CurrentTimeStep, "rudder") ?? 0;
                    return x;
            }}
        public float throttle { get{
                    float x = fgModel?.Table?.getCell(fgModel.CurrentTimeStep, "throttle") ?? 0;
                    return x;
            }}

        public event PropertyChangedEventHandler PropertyChanged;
        private IFlightGearPlayerModel fgModel;
        public JoystickModel(IFlightGearPlayerModel fgModel)
        {
            this.fgModel = fgModel;
            fgModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                if(e.PropertyName == "CurrentTimeStep")
                    NotifyPropertyChanged("aileron", "elevator","throttle","rudder");
            };
            
        }

        private void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
                this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }
}
