using System;
using System.Collections.Generic;
using System.Reflection;
using DLL;
namespace ex1
{
    public class AnomalyDetectorsManager : IAnomalyDetectorsManager
    {
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
