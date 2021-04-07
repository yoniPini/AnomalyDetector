using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL;
using System.ComponentModel;

namespace ex1
{
    public class Point {
        float x; float y;
            public Point (float x, float y) { this.x = x; this.y = y; }
            }
    public interface IAnomalyGraphModel : INotifyPropertyChanged
    {
        //private ITableSeriesListener
        //private IAnomalyDetectionManager
        // con : IAnomalyDetector x;
        List<string> AllFeaturesList { get; }
        int SelectedDetectorIdx { get; set; }        //x.DefaultFeatures; x.Detect; x.Learn;
        bool LoadDetectorFromDll(string fromDllPath);
        string Feature1 { get; set; } // i.e. SelectedFeature
        bool IsFeatur2Exists { get; } // i.e. if the detector has found someother feature,
                                      // =feature2 that it's correlative with feature1
        string Feature2 { get; } // i.e. Most Correlative to Feature1
        List<Point> Feature1Traces { get; } // <timestep[i], value[i]> for i in last 30 timesteps
        List<Point> Feature2Traces { get; } // <timestep[i], value[i]> for i in last 30 timesteps
        List<Point> Feature1And2 { get; } // <x[i],y[i]> for i in last 30 timesteps
        object CorrelationObject { get; }
        string CorrelationObjectType { get; }

        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;
    }
}
