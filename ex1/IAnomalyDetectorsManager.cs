using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL;

namespace ex1
{
    public interface IAnomalyDetectorsManager
    {
        List<IAnomalyDetector> Detectors { get; }
        int CurrentDetectorIdx { get; set; }
        IAnomalyDetector AddAnomalyDetector(string fromDllPath);
        IAnomalyDetector AddAnomalyDetector(IAnomalyDetector ad);
    }
}
