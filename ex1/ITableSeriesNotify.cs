using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ex1
{
    public interface ITableSeriesNotify : INotifyPropertyChanged
    {
        //public TableSeries operator[int i];
        //float this[string x] { get { return 0; } }
        //float get(string feature);
        // note INotifyPropertyChanged may not work well for collection\containers [property which contains properties]
        Dictionary<string, float> FeaturesValue { get; } 
        // changes only within the instance ref:
        List<string> AllFeaturesList { get; }
        //string SelectedFeature { get; set; }

        // sAllFeaturesList { get; } 
        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;
    }
}
