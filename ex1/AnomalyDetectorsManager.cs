using System;
using System.Collections.Generic;
using System.Reflection;
using DLL;
namespace ex1
{
    // class that represents empty detector which return default values = it "finds" no anomalies
    public class EmptyAnomalyDetector : IAnomalyDetector
    {
        public string Name => "Empty Anomaly Detector";

        public string Description => "No anomalies will be given / detect.";

        public string DefaultFeatures { get; set; }
        public float Lin_Reg_Threshold { get; set; }

        public CorrelatedFeatures[] NormalModel => new CorrelatedFeatures[0];

        public AnomalyReport[] LastDetection => new AnomalyReport[0];

        public bool Detect(string pathToCSV)
        {
            return true;
        }

        public void Dispose()
        {
            
        }

        public List<AnomalyReport> LastReports(string asFeature1)
        {
            return new List<AnomalyReport>();
        }

        public bool Learn(string pathToCSV)
        {
            return true;
        }

        public bool Learn(string pathToCSV, float linear_threshold) {
            return true;
        }

        public CorrelatedFeatures MostCorrelativeWith(string feature1)
        {
            return null;
        }
    }

    // manager class for list of IAnomalyDetector
    public class AnomalyDetectorsManager : IAnomalyDetectorsManager
    {
        public static readonly IAnomalyDetector EmptyAnomalyDetector = new EmptyAnomalyDetector();
        private List<IAnomalyDetector> detectors = new List<IAnomalyDetector>();
        public List<IAnomalyDetector> Detectors { get { return this.detectors; } }
        public int CurrentDetectorIdx { get; set; }

        // try to add new instance of detector to the list,
        // according the given dll,and return it, or null if failed
        public IAnomalyDetector AddAnomalyDetector(string fromDllPath)
        {
            try
            {
                // dynamic link
                var dll = Assembly.LoadFile(fromDllPath);
                // get the type of the detector
                var type = dll.GetType("DLL.AnomalyDetector");
                // create new instance (dynamic, because it's .net library it is also "object" type for sure)
                dynamic instance = Activator.CreateInstance(type);
                // try to cast it to IAnomalyDetector and add it to the list
                return AddAnomalyDetector((IAnomalyDetector)instance);
            }
            catch
            {
            }
            return null;
        }
        // add the given detector to the list if it's not null, and return it
        public IAnomalyDetector AddAnomalyDetector(IAnomalyDetector ad)
        {
            if (ad != null)
                this.detectors.Add(ad);
            return ad;
        }
    }
}
