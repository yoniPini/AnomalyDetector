using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ex1
{
    public interface ITableSeriesListener : INotifyPropertyChanged
    {
        //public TableSeries operator[int i];
        //float this[string x] { get { return 0; } }
        float get(string feature);
        // note INotifyPropertyChanged doesnt work well for containers [property which contains properties]
        Dictionary<string, float> Features { get; } 
        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;
    }
}
