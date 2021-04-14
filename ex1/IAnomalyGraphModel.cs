using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL;
using System.ComponentModel;

namespace ex1
{
    // A point with x,y value and field whether it's anomaly or not
    // sometimes x is timestep y is value,
    // sometimes x is feature1 value y and y is feature2 value
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
    // interface of graph model, which set-up all data for its view-model,
    // meaning graph-model gives the information , what features there are,
    // what feature is selected right now,
    // what was its last traces, which feature is most correlative to the first,
    // what anomalies there are and etc.
    // so it will prepare the data to the IOxyViewModel and IAnomalySelectViewModel
    public interface IAnomalyGraphModel : INotifyPropertyChanged
    {
        // implemantation might have:
        // private IFlightGearPlayerModel     to know the data and current timestep
        // private IAnomalyDetectionManager   to get the anomaly reports

        // get all features aviable right now
        List<string> AllFeaturesList { get; }
        int SelectedDetectorIdx { get; set; }

        // return string of "{name}\n{description}" if succeed
        // or the return value as param to String.IsNullOrWhiteSpace will be true is failed
        string LoadDetectorFromDll(string fromDllPath);
        // analayze the current SelectedDetectorIdx
        bool Analyze();

        // i.e. SelectedFeature
        string Feature1 { get; set; } 
        // i.e. if the detector has found someother feature,
        // =feature2 that it's correlative with feature1
        bool IsFeature2Exists { get; } 
        // i.e. Most Correlative to Feature1
        string Feature2 { get; }        
        // <timestep[i], value[i]> for i in last 30 timesteps
        List<SpecialPoint> Feature1Traces { get; } 
        // <timestep[i], value[i]> for i in last 30 timesteps
        List<SpecialPoint> Feature2Traces { get; } 
        // <x[i],y[i]> for i in last 30 timesteps
        List<SpecialPoint> Features1And2 { get; } 
        // e.g. DLL.Circle
        object CorrelationObject { get; }         
        
        // e.g. "Circle"
        string CorrelationObjectType { get; }     
        // timestep of the next anomaly of the choosen feature1 and its correlative feature
        // which it's at least 30 timestep from now (3 sec)
        int NextAnomalyRange { get; }             
        // whether there is next anomaly of the choosen feature1 and its correlative feature
        // which it's at least 30 timestep from now (3 sec)
        bool HasNextAnomalyRange { get; }

        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;
    }
}
