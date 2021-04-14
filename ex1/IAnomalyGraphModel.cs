using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL;
using System.ComponentModel;

namespace ex1
{
    public class SpecialPoint {
        public readonly float x;
        public readonly float y;
        public readonly bool isAnomaly;
        public SpecialPoint (float x, float y, bool isAnomaly) {
            this.x = x;
            this.y = y;
            this.isAnomaly = isAnomaly;
        }
    }
    public interface IAnomalyGraphModel : INotifyPropertyChanged
    {
        //private ITableSeriesListener
        //private IAnomalyDetectionManager
        // con : IAnomalyDetector x;
        List<string> AllFeaturesList { get; }
        int SelectedDetectorIdx { get; set; }        //x.DefaultFeatures; x.Detect; x.Learn;
        string LoadDetectorFromDll(string fromDllPath);
        bool Analyze();
        string Feature1 { get; set; } // i.e. SelectedFeature
        bool IsFeature2Exists { get; } // i.e. if the detector has found someother feature,
                                      // =feature2 that it's correlative with feature1
        string Feature2 { get; } // i.e. Most Correlative to Feature1
        List<SpecialPoint> Feature1Traces { get; } // <timestep[i], value[i]> for i in last 30 timesteps
        List<SpecialPoint> Feature2Traces { get; } // <timestep[i], value[i]> for i in last 30 timesteps
        List<SpecialPoint> Features1And2 { get; } // <x[i],y[i]> for i in last 30 timesteps
        object CorrelationObject { get; }
        
        string CorrelationObjectType { get; }
        int NextAnomalyRange { get; }
        bool HasNextAnomalyRange { get; }

        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;
    }
}
