using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// your dll should contain class named 'AnomalyDetector',
// within the namespace 'DLL',
// which implements the following interface FROM DLL "IAnomalyDetector.dll"
// must contain constructor without params

namespace DLL
{
	
	// default implementation of Circle class
    public class Circle
    {
        public float x, y, r;
        public bool IsContains(double x0, double y0)
        {
            double dx = x - x0;
            double dy = y - y0;
            double r0 = Math.Sqrt(dx * dx + dy * dy);
            double epsilon = 0.0001;
            return r0 <= r + epsilon;
        }
    }
	
	// default implementation of Line class
    public class Line
    {
        public float a, b;
        public float f(float x) { return a * x + b; }
    }
	
	// anomaly data information
    public class AnomalyReport
    {
        public string feature1, feature2;
        public long timeStep; // includes timestep 0 as the first
    }
	
	// learn feature as correlative means there is instance of CorrelatedFeatures,
	// which describes the correlation
    public class CorrelatedFeatures
    {
        public string feature1, feature2;
        public float correlation;
        public string type; // "Circle" \ "Line" etc.
        public object objectOfCorrelation;
    }
	
	
    public interface IAnomalyDetector : IDisposable
    {
		// Name of detector/algorithim
        string Name { get; }									
        string Description { get; }		
		
		// Default Features line. e.g  "speed,velocity,height",
		// which will be in used if the csv of learning / detection doesn't contains one.
        string DefaultFeatures { get; set; }					
		// threshold of the min abs value the pearson between 2 features need to be to consider as liner_regression correlative														
        float Lin_Reg_Threshold { get; set; }
		
		// learn correlative feature, using the Lin_Reg_Threshold property, returns false if was error
        bool Learn(string pathToCSV);
		// learn correlative feature, using the linear_threshold param, returns false if was error
        bool Learn(string pathToCSV, float linear_threshold);
		
		// after learning, get all correlations, if weren't return new CorrelatedFeatures[0];
        CorrelatedFeatures[] NormalModel { get; }
		// get correlation according to feature name, as being the left feature (if A-B correlative B-A probably won't be)
		// if there is no correlation return null
        CorrelatedFeatures MostCorrelativeWith(string feature1);
		
		// detect anomalies according the normal model(correlative features), returns false if was error
        bool Detect(string pathToCSV);
		// get all anomalies from last detection, if there wasn't return new AnomalyReport[0];
        AnomalyReport[] LastDetection { get; }
		// get all anomalies where asFeature1 was the left feature
		// if there wasn't, return new List<AnomalyReport>();
        List<AnomalyReport> LastReports(string asFeature1);


        // IDisposable 
        // void Dispose();
		// ^^^Get prepare to ANOTHER learn, meaning normal use of this IAnomalyDetector will be
		
		/*
			IAnomalyDetector ad = new ...
			ad.DefaultFeatures = ...
			ad.Lin_Reg_Threshold = ...
			while (...) {
				ad.Dispose()
				ad.Learn(...)
				ad.Detect(...)
				using of NormalModel, MostCorrelativeWith, LastDetection, LastReports
				// next iteration
			}
		*/
    }
}

