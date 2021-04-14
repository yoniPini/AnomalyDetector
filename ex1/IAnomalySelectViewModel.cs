using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ex1
{

    public interface IAnomalySelectViewModel : INotifyPropertyChanged
    {
        List<string> VM_AllFeaturesList { get; }
        int VM_SelectedDetectorIdx { get; set; }        //x.DefaultFeatures; x.Detect; x.Learn;
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
