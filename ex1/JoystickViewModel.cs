using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace ex1
{
    public class JoystickViewModel : IJoystickViewModel
    {
        // view model -> view  :
        public float VM_throttle { get { return model.throttle; } }
        public float VM_rudder { get { return model.rudder; } }


        // view -> view model  :
        public double VM_Height_BigCircle { get; set; }
        public double VM_Height_SmallCircle { get; set; }
        public double VM_Width_SmallCircle { get; set; }
        public double VM_Width_BigCircle { get; set; }
        public Thickness VM_Margin_BigCircle { get; set; }

        // view model -> view  :
        public Thickness VM_Margin_SmallCircle {
            get {
                double halfHeight = VM_Height_BigCircle / 2;
                double halfWidth = VM_Width_BigCircle / 2;
                double startY = VM_Margin_BigCircle.Top + halfHeight - VM_Height_SmallCircle / 2;
                double startX = VM_Margin_BigCircle.Left + halfWidth - VM_Width_SmallCircle / 2;
                double addX = model.aileron * halfWidth;
                double addY = model.elevator * halfHeight;
                double r = Math.Max(addX, addY);
                double angle = Math.Atan(addX / addY);
                if (Double.IsNaN(angle)) angle = 0;
                double xSign = addX < 0 ? -1 : 1;
                double ySign = addY < 0 ? -1 : 1;
                double x = startX + xSign * Math.Cos(angle) * r;
                double y = startY + ySign * Math.Sin(angle) * r;
                return new Thickness(x, y, 0, 0); 
            } }
        public event PropertyChangedEventHandler PropertyChanged;
        private IJoystickModel model;
        public JoystickViewModel(IJoystickModel model)
        { 
            this.model = model;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged("VM_" + e.PropertyName);
                if (e.PropertyName == "aileron" || e.PropertyName == "elevator")
                {
                    NotifyPropertyChanged("VM_Margin_SmallCircle");
                }

            };
        }
        private void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
                this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }
}
