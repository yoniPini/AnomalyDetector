using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ex1
{
    // interface to select view which get data (from IAnomalyGraphModel)
    // and prepare these data to the view
    // the data is about how much features there are, which one is selected and so on
    // On the other hand, this interface will change the IAnomalyGraphModel selected feature [feature 1]
    public interface IAnomalySelectViewModel : INotifyPropertyChanged
    {
        List<string> VM_AllFeaturesList { get; }
        int VM_SelectedDetectorIdx { get; set; }
        string LoadDetectorFromDll(string fromDllPath);
        bool Analyze();
        string VM_Feature1 { get; set; } // i.e. SelectedFeature
        bool VM_IsFeature2Exists { get; }
        string VM_Feature2 { get; }
        string VM_CorrelationObjectType { get; }
        int VM_NextAnomalyRange { get; }
        bool VM_HasNextAnomalyRange { get; }

        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;
    }
}
