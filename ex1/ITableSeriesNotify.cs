using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ex1
{
    // interface of notifier(via PropertyChanged event) of values of current row in TableSeries
    // (meaning that the row has changed , like for example time passed)
    public interface ITableSeriesNotify : INotifyPropertyChanged
    {
        // note INotifyPropertyChanged may not work well for collection\containers [property which contains properties]
        Dictionary<string, float> FeaturesValue { get; } 

        // changes only within the instance ref:
        List<string> AllFeaturesList { get; }
        
        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;
    }
}
