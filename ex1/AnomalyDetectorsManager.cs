using System;
using System.Collections.Generic;
using System.Reflection;
using DLL;
namespace ex1
{
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
    public class AnomalyDetectorsManager : IAnomalyDetectorsManager
    {
        public static readonly IAnomalyDetector EmptyAnomalyDetector = new EmptyAnomalyDetector();
        private List<IAnomalyDetector> detectors = new List<IAnomalyDetector>();
        public List<IAnomalyDetector> Detectors { get { return this.detectors; } }
        public int CurrentDetectorIdx { get; set; }
        public IAnomalyDetector AddAnomalyDetector(string fromDllPath)
        {
            try
            {
                var dll = Assembly.LoadFile(fromDllPath);
                var type = dll.GetType("DLL.AnomalyDetector");
                dynamic instance = Activator.CreateInstance(type);
                return AddAnomalyDetector((IAnomalyDetector)instance);
            }
            catch
            {
            }
            return null;
        }
        public IAnomalyDetector AddAnomalyDetector(IAnomalyDetector ad)
        {
            if (ad != null)
                this.detectors.Add(ad);
            return ad;
        }
    }
}
