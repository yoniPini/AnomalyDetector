using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL;
using System.ComponentModel;

namespace ex1
{
    public class AnomalyGraphModel : IAnomalyGraphModel
    {
        //private ITableSeriesListener
        //private IAnomalyDetectionManager
        // con : IAnomalyDetector x;
        private IFlightGearPlayerModel fgModel;
        private IAnomalyDetectorsManager detectorsManager;
        public AnomalyGraphModel(IFlightGearPlayerModel fgModel)
        {
            this.fgModel = fgModel;
            fgModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName == "AllFeaturesList") NotifyPropertyChanged("AllFeaturesList");
                if (e.PropertyName == "CurrentTimeStep") 
                    NotifyPropertyChanged("Feature1Traces", "Feature2Traces", "Features1And2");
                if (e.PropertyName == "IsPowerOn") this.updatedDetectors.Clear();
            };
            this.detectorsManager = new AnomalyDetectorsManager();
            this.detectorsManager.AddAnomalyDetector(AnomalyDetectorsManager.EmptyAnomalyDetector);
            this.detectorsManager.CurrentDetectorIdx = 0;
        }
        public List<string> AllFeaturesList { get { return this.fgModel.AllFeaturesList; } }

       // private int m_selectedDetectorIndex = -1;
        public int SelectedDetectorIdx
        {
            get { return detectorsManager.CurrentDetectorIdx; }
            set {
                if (detectorsManager.CurrentDetectorIdx == value || value < 0 || value >= detectorsManager.Detectors.Count)
                    return;
                detectorsManager.CurrentDetectorIdx = value;

                NotifyPropertyChanged("SelectedDetectorIdx", "Feature1", "IsFeature2Exists",
                               "Feature2", "CorrelationObject", "CorrelationObjectType");
                NotifyPropertyChanged("Feature1Traces", "Feature2Traces", "Features1And2");
                }
        }
        /*
         */

        //notify...    //x.DefaultFeatures; x.Detect; x.Learn;

        public event PropertyChangedEventHandler PropertyChanged;

        public string LoadDetectorFromDll(string fromDllPath) { 
            var d = this.detectorsManager.AddAnomalyDetector(fromDllPath);
            if (d == null) return "";
            return d.Name + " :\n" + d.Description;
        }
        private List<int> updatedDetectors = new List<int>();
        public bool Analyze()
        {
            if (updatedDetectors.Contains(SelectedDetectorIdx)) return true;
            var d = CurrDetector;
            d.DefaultFeatures = Utils.ParseFeaturesLine(this.fgModel.XML_Path);
            if ( d.Learn(this.fgModel.LearnCsv_Path) && d.Detect(this.fgModel.TestCsv_Path))
            {
                updatedDetectors.Add(SelectedDetectorIdx);
                return true;
            }
            return false;
        }

        private string feature1 = "";
        private HashSet<long> currentFeature1AnomaliesTimeStep = new HashSet<long>();
        public string Feature1 { get { return feature1; }
            set {
                 if (value == feature1 || !AllFeaturesList.Contains(value)) return;
                 feature1 = value;

                 List<AnomalyReport> anomalyList;
                if (SelectedDetectorIdx < 0 || CurrDetector == null)
                    anomalyList = new List<AnomalyReport>();
                else
                    anomalyList = CurrDetector.LastReports(feature1);
                currentFeature1AnomaliesTimeStep = new HashSet<long>();
                foreach (var anomaly in anomalyList)
                    currentFeature1AnomaliesTimeStep.Add(anomaly.timeStep);
                
                NotifyPropertyChanged("Feature1", "IsFeature2Exists",
                                "Feature2", "CorrelationObject", "CorrelationObjectType");
                NotifyPropertyChanged("Feature1Traces", "Feature2Traces", "Features1And2");
            }
        } // i.e. SelectedFeature //notify..
        /*
         */
        private CorrelatedFeatures GetCurrentCorrelatedFeatures()
        {
            if (String.IsNullOrWhiteSpace(Feature1) || CurrDetector == null) return null;
            //try
            //{
                return CurrDetector.MostCorrelativeWith(Feature1);
            //}
            //catch
            //{
            //    return null;
            //}
        }
        public bool IsFeature2Exists { 
            get {
                return GetCurrentCorrelatedFeatures() != null;
            } } // i.e. if the detector has found someother feature,
                                             // =feature2 that it's correlative with feature1
        public string Feature2
        {
            get
            {
                return GetCurrentCorrelatedFeatures()?.feature2 ?? "";
            }
        
        } // i.e. Most Correlative to Feature1
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
                for (int i = Math.Max(fgModel.CurrentTimeStep - 30, 0); i < fgModel.CurrentTimeStep; i++)
                {
                    float value = fgModel.Table.getCell(i, Feature1);
                    bool isAnomaly = currentFeature1AnomaliesTimeStep.Contains(i);
                    list.Add(new SpecialPoint(i, value, isAnomaly));
                }
                return list;
            }} // <timestep[i], value[i]> for i in last 30 timesteps
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
                for (int i = Math.Max(fgModel.CurrentTimeStep - 30, 0); i < fgModel.CurrentTimeStep; i++)
                {
                    float value = fgModel.Table.getCell(i, Feature2);
                    bool isAnomaly = currentFeature1AnomaliesTimeStep.Contains(i);
                    list.Add(new SpecialPoint(i, value, isAnomaly));
                }
                return list;
            }
        } // <timestep[i], value[i]> for i in last 30 timesteps
        private IAnomalyDetector CurrDetector
        {
            get {
                if (SelectedDetectorIdx < 0 || SelectedDetectorIdx >= detectorsManager.Detectors.Count) return null;
                return this.detectorsManager.Detectors[this.detectorsManager.CurrentDetectorIdx];
            }
            
        }
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
            for (int i = Math.Max(fgModel.CurrentTimeStep - 30, 0); i < fgModel.CurrentTimeStep; i++)
            {
                float valueF1 = fgModel.Table.getCell(i, Feature1);
                float valueF2 = fgModel.Table.getCell(i, Feature2);
                bool isAnomaly = currentFeature1AnomaliesTimeStep.Contains(i);
                list.Add(new SpecialPoint(valueF1, valueF2, isAnomaly));
            }
            return list;
        }
    }
 // <x[i],y[i]> for i in last 30 timesteps
public object CorrelationObject { 
            get {
                if (CurrDetector == null || String.IsNullOrWhiteSpace(Feature1)) return null;
                return CurrDetector.MostCorrelativeWith(Feature1).objectOfCorrelation;
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
