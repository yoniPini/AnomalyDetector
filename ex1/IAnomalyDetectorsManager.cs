using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL;

namespace ex1
{
    // Interface to anomaly manager, which has list of IAnomalyDetector
    // and can add to it more detector / load it from suitable dll.
    // [DLL.AnomalyDetector with empty constructor and implements IAnomalyDetector (from the dll)]
    public interface IAnomalyDetectorsManager
    {
        List<IAnomalyDetector> Detectors { get; }
        int CurrentDetectorIdx { get; set; }
        // load new instance from the given file, or return null if failed.
        IAnomalyDetector AddAnomalyDetector(string fromDllPath);
        IAnomalyDetector AddAnomalyDetector(IAnomalyDetector ad);
    }
}
