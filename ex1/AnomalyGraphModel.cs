using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL;
using System.ComponentModel;

namespace ex1
{
    // class which get data of timesteps from IFlightGearPlayerModel
    // and anlyze anomaly reporty via IAnomalyDetectorsManager
    // and produce the calculated data to IOxyViewModel (for graph draw)
    // and to IAnomalySeletViewModel for the ability to choose feature amoung the aviable features
    
    // First element is added automatically= Empty Anomaly detector [doesn't report any anomaly],
    // (in index 0)
    public class AnomalyGraphModel : IAnomalyGraphModel
    {
        private IFlightGearPlayerModel fgModel;
        private IAnomalyDetectorsManager detectorsManager;
        public AnomalyGraphModel(IFlightGearPlayerModel fgModel)
        {
            this.fgModel = fgModel;
            fgModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName == "AllFeaturesList") NotifyPropertyChanged("AllFeaturesList");
                if (e.PropertyName == "CurrentTimeStep") 
                    NotifyPropertyChanged("Feature1Traces", "Feature2Traces", "Features1And2",
                         "NextAnomalyRange", "HasNextAnomalyRange");
                // power on-off reset the 'updated' detectors
                if (e.PropertyName == "IsPowerOn") this.updatedDetectors.Clear(); 
            };
            // add detector manager with first element as Empty Anomaly detector [doesn't report any anomaly]
            this.detectorsManager = new AnomalyDetectorsManager();
            this.detectorsManager.AddAnomalyDetector(AnomalyDetectorsManager.EmptyAnomalyDetector);
            this.detectorsManager.CurrentDetectorIdx = 0;
        }
        public List<string> AllFeaturesList { get { return this.fgModel.AllFeaturesList; } }

        // get or set the detector (via idx) which the other properties of this class refer to
        public int SelectedDetectorIdx
        {
            get { return detectorsManager.CurrentDetectorIdx; }
            set {
                if (detectorsManager.CurrentDetectorIdx == value || value < 0 || value >= detectorsManager.Detectors.Count)
                {
                    this.Feature1 = this.Feature1; // make Feature1 to "refresh" its value
                    return;
                }
                detectorsManager.CurrentDetectorIdx = value;
                this.Feature1 = this.Feature1;              // make Feature1 to "refresh" its value
                NotifyPropertyChanged("SelectedDetectorIdx");
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        // add detector to detectorManager and return "{name}\n{description}" if succeeded,
        // empty string if failed
        public string LoadDetectorFromDll(string fromDllPath) { 
            var d = this.detectorsManager.AddAnomalyDetector(fromDllPath);
            if (d == null) return "";
            return d.Name + " :\n" + d.Description;
        }
        // list of detectors that the learning and detecting are already update for the current xml+train_csv+test_csv
        private List<int> updatedDetectors = new List<int>();

        // make the selected detector :  learn the normal model ,and detect anomalies
        public bool Analyze()
        {
            if (updatedDetectors.Contains(SelectedDetectorIdx)) return true;
            var d = CurrDetector;
            d.Dispose();
            d.DefaultFeatures = Utils.ParseFeaturesLine(this.fgModel.XML_Path);
            if ( d.Learn(this.fgModel.LearnCsv_Path) && d.Detect(this.fgModel.TestCsv_Path))
            {
                updatedDetectors.Add(SelectedDetectorIdx);
                return true;
            }
            return false;
        }

        private string feature1 = "";
        // if the "current" feature has anomaly in timestep x iff the above set contains x
        private HashSet<long> currentFeature1AnomaliesTimeStep = new HashSet<long>();
        public string Feature1 { get { return feature1; }
            set {
                // check selected feature exist. any way "update" the listeners to PropertyChanged
                if (!AllFeaturesList.Contains(value)) {
                    feature1 = "";
                    currentFeature1AnomaliesTimeStep = new HashSet<long>();
                    NotifyPropertyChanged("Feature1", "IsFeature2Exists", "NextAnomalyRange", "HasNextAnomalyRange",
                                "Feature2", "CorrelationObject", "CorrelationObjectType");
                    NotifyPropertyChanged("Feature1Traces", "Feature2Traces", "Features1And2");
                    return; 
                }
                 feature1 = value;

                // update the currentFeature1AnomaliesTimeStep due the selected feature
                // even if it's the prev value, in order to be updated (see  SelectedDetectorIdx  setter),
                // because maybe the whole test_csv was changed
                List<AnomalyReport> anomalyList;
                if (SelectedDetectorIdx < 0 || CurrDetector == null)
                    anomalyList = new List<AnomalyReport>();
                else
                    anomalyList = CurrDetector.LastReports(feature1);
                currentFeature1AnomaliesTimeStep = new HashSet<long>();
                foreach (var anomaly in anomalyList)
                    currentFeature1AnomaliesTimeStep.Add(anomaly.timeStep);
                
                NotifyPropertyChanged("Feature1", "IsFeature2Exists", "NextAnomalyRange", "HasNextAnomalyRange",
                                "Feature2", "CorrelationObject", "CorrelationObjectType");
                NotifyPropertyChanged("Feature1Traces", "Feature2Traces", "Features1And2");
            }
        }

        // get the next anomaly that apper after
        // currtimestep + 30 timesteps == currtimestep + 3 sec == currtimestep + 3* hz
        public int NextAnomalyRange { 
            get
            {
                var x = currentFeature1AnomaliesTimeStep;
                var maxStep = fgModel.MaxTimeStep;
                int i = (int)(fgModel.CurrentTimeStep + 3 * fgModel.Const_OriginalHz);
                for (; i <= maxStep; i++)
                    if (x.Contains(i)) return i;
                return -1;
            } }

        public bool HasNextAnomalyRange => NextAnomalyRange != -1;
        private CorrelatedFeatures GetCurrentCorrelatedFeatures()
        {
            if (String.IsNullOrWhiteSpace(Feature1) || CurrDetector == null) return null;
                return CurrDetector.MostCorrelativeWith(Feature1);
        }
        
        // i.e. if the detector has found someother feature,
        // =feature2 that it's correlative with feature1
        public bool IsFeature2Exists { 
            get {
                return GetCurrentCorrelatedFeatures() != null;
            } } 
        
        // i.e. Most Correlative to Feature1
        public string Feature2
        {
            get
            {
                return GetCurrentCorrelatedFeatures()?.feature2 ?? "";
            }
        
        } 

        // get [currentTimeStep-30 , currentTimeStep] points (special point has also field if it's anomaly)
        // x is timestep, y is value of feature 1
        public List<SpecialPoint> Feature1Traces { 
            get
            {
                if (String.IsNullOrWhiteSpace(Feature1)) return new List<SpecialPoint>();
                List<SpecialPoint> list = new List<SpecialPoint>();
                List<AnomalyReport> anomalyList;
                if (SelectedDetectorIdx < 0 || CurrDetector == null) 
                    anomalyList = new List<AnomalyReport>();
                else
                    anomalyList = CurrDetector.LastReports(Feature1);
                for (int i = Math.Max(fgModel.CurrentTimeStep - 30, 0); i <= fgModel.CurrentTimeStep; i++)
                {
                    float value = fgModel.Table.getCell(i, Feature1);
                    bool isAnomaly = currentFeature1AnomaliesTimeStep.Contains(i);
                    list.Add(new SpecialPoint(i, value, isAnomaly));
                }
                return list;
            }}

        // get [currentTimeStep-30 , currentTimeStep] points (special point has also field if it's anomaly)
        // x is timestep, y is value of feature 2
        public List<SpecialPoint> Feature2Traces
        {
            get
            {
                if (!IsFeature2Exists) return new List<SpecialPoint>();
                List<SpecialPoint> list = new List<SpecialPoint>();
                List<AnomalyReport> anomalyList;
                if (SelectedDetectorIdx < 0 || CurrDetector == null)
                    anomalyList = new List<AnomalyReport>();
                else
                    anomalyList = CurrDetector.LastReports(Feature1);
                for (int i = Math.Max(fgModel.CurrentTimeStep - 30, 0); i <= fgModel.CurrentTimeStep; i++)
                {
                    float value = fgModel.Table.getCell(i, Feature2);
                    bool isAnomaly = currentFeature1AnomaliesTimeStep.Contains(i);
                    list.Add(new SpecialPoint(i, value, isAnomaly));
                }
                return list;
            }
        }
        private IAnomalyDetector CurrDetector
        {
            get {
                if (SelectedDetectorIdx < 0 || SelectedDetectorIdx >= detectorsManager.Detectors.Count) return null;
                return this.detectorsManager.Detectors[this.detectorsManager.CurrentDetectorIdx];
            }
            
        }

    
    // < x=feature1 values[i], y=feature2 values[i], ?isAnomaly > for i in last 30 timesteps
    public List<SpecialPoint> Features1And2
    {
        get
        {
            if (!IsFeature2Exists) return new List<SpecialPoint>();
            List<SpecialPoint> list = new List<SpecialPoint>();
            List<AnomalyReport> anomalyList;
            if (SelectedDetectorIdx < 0 || CurrDetector == null)
                anomalyList = new List<AnomalyReport>();
            else
                anomalyList = CurrDetector.LastReports(Feature1);
            for (int i = Math.Max(fgModel.CurrentTimeStep - 30, 0); i <= fgModel.CurrentTimeStep; i++)
            {
                float valueF1 = fgModel.Table.getCell(i, Feature1);
                float valueF2 = fgModel.Table.getCell(i, Feature2);
                bool isAnomaly = currentFeature1AnomaliesTimeStep.Contains(i);
                list.Add(new SpecialPoint(valueF1, valueF2, isAnomaly));
            }
            return list;
        }
    }
        public object CorrelationObject { 
            get {
                if (CurrDetector == null || String.IsNullOrWhiteSpace(Feature1)) return null;
                return CurrDetector.MostCorrelativeWith(Feature1)?.objectOfCorrelation;
            }
        }
        public string CorrelationObjectType {
            get
            {
                if (CurrDetector == null || String.IsNullOrWhiteSpace(Feature1)) return null;
                return CurrDetector.MostCorrelativeWith(Feature1).type;
            }
        }

        private void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
                this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }
}
