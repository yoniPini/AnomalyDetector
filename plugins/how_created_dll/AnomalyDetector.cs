using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
namespace DLL
{
    // A class which will be runtime-library, implements IAnomalyDetector
    // The class is based on the project from last semester which became ".\\anomaly_detector_helper"
    // Basically, the diffrent between the inner detection method, as long as the extends SimpleAnomalyDetection.cpp
    // (and included in the ".\\anomaly_detector_helper") is just differ in which type we get from the DefaultDLL_AnomalyDetector method.
    // or DefaultDLL_Threshold that might be different. [DefaultDLL_ methots are the diffrents]

    // This Class is 'wrapper' to the cpp SimpleAnomalyDetector class
    public class AnomalyDetector : IAnomalyDetector
    {

        /////////////////////////////////////////////////////////////////////////////////////////
        [DllImport(".\\anomaly_detector_helper")]
        public static extern float getCircleX_cpp(IntPtr c);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern float getCircleY_cpp(IntPtr c);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern float getCircleR_cpp(IntPtr c);

        [DllImport(".\\anomaly_detector_helper")]
        public static extern void del_SimpleAnomalyDetector(IntPtr ad);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern IntPtr getSimpleAnomalyDetector_cpp(string csvfile, float linear_threshold);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern float Lin_Reg_Simple_Default_Threshold();
        [DllImport(".\\anomaly_detector_helper")]
        public static extern IntPtr getHybridAnomalyDetector_cpp(string csvfile, float linear_threshold);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern float Lin_Reg_Hybrid_Default_Threshold();
        [DllImport(".\\anomaly_detector_helper")]
        public static extern void setLin_Reg_Threshold(IntPtr ad, float p);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern float getLin_Reg_Threshold(IntPtr ad);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern void del_AnomalyReportArray(IntPtr ar);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern IntPtr getAnomalyReport_cpp(IntPtr ad, string csvfile);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern int getAnomalyReportLastSize_cpp(IntPtr ad);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern IntPtr getFeaturesInReport(IntPtr ar, int idx); //should be converted to string
        [DllImport(".\\anomaly_detector_helper")]
        public static extern int getTimeStepInReport(IntPtr ar, int idx);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern int getNormalModelSize(IntPtr ad);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern IntPtr getNormalModelFeature1(IntPtr ad, int idx); //should be converted to string
        [DllImport(".\\anomaly_detector_helper")]
        public static extern IntPtr getNormalModelFeature2(IntPtr ad, int idx); //should be converted to string
        [DllImport(".\\anomaly_detector_helper")]
        public static extern float getNormalModelCorrelation(IntPtr ad, int idx);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern bool getNormalModelIsLinearReg(IntPtr ad, int idx);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern bool getNormalModelIsMinimalCircle(IntPtr ad, int idx);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern IntPtr getNormalModelgetMinimalCircle(IntPtr ad, int idx);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern IntPtr getNormalModelgetLinearReg(IntPtr ad, int idx);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern float getA_Line(IntPtr l);
        [DllImport(".\\anomaly_detector_helper")]
        public static extern float getB_Line(IntPtr l);
        /////////////////////////////////////////////////////////////////////////////////////////
        
        private IntPtr AnomalyDetectorPtr = IntPtr.Zero;
        private Dictionary<string, CorrelatedFeatures> mostCorrelative = new Dictionary<string, CorrelatedFeatures>();
        private CorrelatedFeatures[] correlatedFeaturesArray = new CorrelatedFeatures[0];
        private AnomalyReport[] lastReport = new AnomalyReport[0];
        private Dictionary<string, List<AnomalyReport>> reportsOfBeingFeature1 = new Dictionary<string, List<AnomalyReport>>();
        public string DefaultFeatures { get; set; }
        public CorrelatedFeatures[] NormalModel { get { return this.correlatedFeaturesArray; } }
        private float threshold =  DefaultDLL_Threshold();
        public float Lin_Reg_Threshold { 
            get { return this.threshold; }
            set {
                if (this.AnomalyDetectorPtr != IntPtr.Zero)
                    setLin_Reg_Threshold(this.AnomalyDetectorPtr, value);
                this.threshold = value;
            }
        }
        public string Name { get { return DefaultDLL_Name(); } }
        public string Description { get { return DefaultDLL_Description(); } }

        //////////////////////////////////////////////////////////////////////
        private static string DefaultDLL_Name()
        {
            //return "SimpleAnomalyDetector";
            return "HybridAnomalyDetector";
        }
        private static string DefaultDLL_Description()
        {
            return "Detect anomalies which are too far from the linear regresion.\n" +
                   "If two features abs correlation value is more than the abs value of the threshold ( default pearson is 0.9 )." //;
               + "\nIf it's smaller than the linear reg threshold but larger then 0.5 than use minimal circle detection.";
        }
        private static float DefaultDLL_Threshold()
        {
            //return Lin_Reg_Simple_Default_Threshold();
            return Lin_Reg_Hybrid_Default_Threshold();
        }
        private static IntPtr DefaultDLL_AnomalyDetector(string csv, float threshold)
        {
            //return getSimpleAnomalyDetector_cpp(csv, threshold);
            return getHybridAnomalyDetector_cpp(csv, threshold);
        }
        //////////////////////////////////////////////////////////////////////
        public AnomalyReport[] LastDetection { get { return this.lastReport; } }
        
        public bool Learn(string pathToCSV)
        {
            return Learn(pathToCSV, this.threshold);
        }
        public bool Learn(string pathToCSV, float linear_threshold)
        {
            this.threshold = linear_threshold;
            string pathToCSVtmp = pathToCSV + ".tmp";
            try {
                using (FileStream inFile = File.OpenRead(pathToCSV)) {
                    using (FileStream outFile = File.Open(pathToCSVtmp, FileMode.Create)) {
                        var input = new StreamReader(inFile);
                        var output = new StreamWriter(outFile);
                        string firstLine = input.ReadLine();
                        char c = firstLine[0];
                        if (Char.IsDigit(c) || c == '.')
                            output.Write(DefaultFeatures + "\n");
                        output.Write(firstLine + "\n");
                        while (!input.EndOfStream)
                        {
                            output.Write(input.ReadLine() + "\n");
                            output.Flush();
                        }
                    }
                }
                
                this.AnomalyDetectorPtr = DefaultDLL_AnomalyDetector(pathToCSVtmp, linear_threshold); //simple or hybrid
                IntPtr ad = this.AnomalyDetectorPtr;
                this.correlatedFeaturesArray = new CorrelatedFeatures[getNormalModelSize(ad)];

                for (int i = 0; i < this.correlatedFeaturesArray.Length; i++) {
                    this.correlatedFeaturesArray[i] = new CorrelatedFeatures();
                    this.correlatedFeaturesArray[i].feature1 = 
                        System.Runtime.InteropServices.Marshal.PtrToStringAnsi(getNormalModelFeature1(ad, i));
                    this.correlatedFeaturesArray[i].feature2 = 
                        System.Runtime.InteropServices.Marshal.PtrToStringAnsi(getNormalModelFeature2(ad, i));
                    this.correlatedFeaturesArray[i].correlation = getNormalModelCorrelation(ad, i);
                    
                    if (getNormalModelIsLinearReg(ad, i)) {
                        IntPtr cppLine = getNormalModelgetLinearReg(ad, i);
                        Line l = new Line();
                        l.a = getA_Line(cppLine);
                        l.b = getB_Line(cppLine);
                        this.correlatedFeaturesArray[i].objectOfCorrelation = l;
                        this.correlatedFeaturesArray[i].type = "Line";
                    }
                    if (getNormalModelIsMinimalCircle(ad, i)) {
                        IntPtr cppCircle = getNormalModelgetMinimalCircle(ad, i);
                        Circle c = new Circle();
                        c.x = getCircleX_cpp(cppCircle);
                        c.y = getCircleY_cpp(cppCircle);
                        c.r = getCircleR_cpp(cppCircle);
                        this.correlatedFeaturesArray[i].objectOfCorrelation = c;
                        this.correlatedFeaturesArray[i].type = "Circle";
                    }
                    if (!mostCorrelative.ContainsKey(this.correlatedFeaturesArray[i].feature1))
                        mostCorrelative.Add(this.correlatedFeaturesArray[i].feature1, this.correlatedFeaturesArray[i]);
                }
                return true;
            }
            catch { return false; }
        }
        public CorrelatedFeatures MostCorrelativeWith(string feature1) {
            if (!mostCorrelative.ContainsKey(feature1))
                return null;
            return mostCorrelative[feature1];
        }
        public bool Detect(string pathToCSV)
        {
            string pathToCSVtmp = pathToCSV + ".tmp";
            try
            {
                using (FileStream inFile = File.OpenRead(pathToCSV))
                {
                    using (FileStream outFile = File.Open(pathToCSVtmp, FileMode.Create))
                    {
                        var input = new StreamReader(inFile);
                        var output = new StreamWriter(outFile);
                        string firstLine = input.ReadLine();
                        char c = firstLine[0];
                        if (Char.IsDigit(c) || c == '.')
                            output.Write(DefaultFeatures + "\n");
                        output.Write(firstLine + "\n");
                        while (!input.EndOfStream)
                        {
                            output.Write(input.ReadLine() + "\n");
                            output.Flush();
                        }
                    }
                }
                IntPtr ad = this.AnomalyDetectorPtr;
                IntPtr reports = getAnomalyReport_cpp(ad, pathToCSVtmp);
                this.lastReport = new AnomalyReport[getAnomalyReportLastSize_cpp(ad)];
                for (int i = 0; i < this.lastReport.Length; i++)
                {
                    this.lastReport[i] = new AnomalyReport();
                    string features = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(getFeaturesInReport(reports, i));
                    this.lastReport[i].feature1 = features.Split(',')[0];
                    this.lastReport[i].feature2 = features.Split(',')[1];
                    this.lastReport[i].timeStep = getTimeStepInReport(reports, i);

                    if (!this.reportsOfBeingFeature1.ContainsKey(this.lastReport[i].feature1))
                        this.reportsOfBeingFeature1.Add(this.lastReport[i].feature1, new List<AnomalyReport>());
                    this.reportsOfBeingFeature1[this.lastReport[i].feature1].Add(this.lastReport[i]);
                }
                del_AnomalyReportArray(reports);
                return true;
            }
            catch { return false; }
        }
        public List<AnomalyReport> LastReports(string asFeature1)
        {
            if (!this.reportsOfBeingFeature1.ContainsKey(asFeature1))
                return new List<AnomalyReport>();
            return this.reportsOfBeingFeature1[asFeature1];
        }
        // IDisposable implementation:
        public void Dispose()
        {
            del_SimpleAnomalyDetector(this.AnomalyDetectorPtr);
            this.AnomalyDetectorPtr = IntPtr.Zero;
            this.mostCorrelative = new Dictionary<string, CorrelatedFeatures>();
            this.correlatedFeaturesArray = new CorrelatedFeatures[0];
            this.lastReport = new AnomalyReport[0];
            this.reportsOfBeingFeature1 = new Dictionary<string, List<AnomalyReport>>();
            this.threshold = DefaultDLL_Threshold();
        }
        
    }
}
