using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Timers;

namespace ex1
{
    public class FlightGearPlayerModel : IFlightGearPlayerModel
    {
        public Dictionary<string, float> FeaturesValue { get {
                var d = new Dictionary<string, float>();
                foreach (string s in AllFeaturesList)
                    if (!d.ContainsKey(s))
                        d.Add(s, Table.getCell(CurrentTimeStep, s));
                return d;
            } }
        
        public List<string> AllFeaturesList { get {
                return Table.featuresStrAsList;
            } }


        public double Const_OriginalHz { get { return 10.00; } }
        // only binding from model view -> model :
        public string FG_Path { get; set; }
        public string XML_Path { get; set; }
        public string LearnCsv_Path { get; set; }
        public string TestCsv_Path { get; set; }



        private bool isRunningNow;
        public bool IsRunning { get { return isRunningNow; } set {
                if (isRunningNow == value) return;
                if (value)
                    Play();
                else
                    Pause();
                this.isRunningNow = value;
                NotifyPropertyChanged("IsRunning", "IsPaused");
            } }
        public bool IsPaused { get { return !IsRunning; } set { IsRunning = !value; } } // opposite of the above

        private int currTimeStep;
        public int CurrentTimeStep { get { return currTimeStep; } 
            set { 
                currTimeStep = Math.Min(MaxTimeStep, Math.Max(0, value)); 
                NotifyPropertyChanged("CurrentTimeStep", "CurrentTimeInStr", "FeaturesValue"); } } // 95 for example
        public int MaxTimeStep { get { 
                if(ts != null)
                    return ts.RowsLength - 1;
                return 0;
            } } // including the return value
        private String TwoDigitsRepr(int x)
        {
            if (x == 0) return "00";
            if (x >= 10) return "" + x;
            return "0" + x;
        }
        public String CurrentTimeInStr { get {
                int curr = CurrentTimeStep;
                int currSec = curr / (int)Const_OriginalHz;
                int currMin = currSec / 60;
                int currHour = currMin / 60;
                currSec = currSec % 60;
                currMin = currMin % 60;
                return TwoDigitsRepr(currHour) + ":" + TwoDigitsRepr(currMin) + ":" + TwoDigitsRepr(currSec);
            } } // "00:01:35 for example"
        private double speed;
        public double SpeedTimes { get { return speed; } set { speed = value; UpdateSpeed(); NotifyPropertyChanged("SpeedTimes"); } }// 1.5 = "x1.5" speed
        
        // INotifyPropertyChanged:
        public event PropertyChangedEventHandler PropertyChanged;
        private FgAdapter fgAdapter = null;
        private Timer timer = null; 
        private TableSeries ts = null; 
        public TableSeries Table { get { return ts; } }
        public FlightGearPlayerModel()
        {
            this.FG_Path = "";
            this.XML_Path = "";
            this.LearnCsv_Path = "";
            this.TestCsv_Path = "";
            this.IsRunning = false;
            this.CurrentTimeStep = 0;
            this.SpeedTimes = 1;
        }
        private void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
                this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
        private void UpdateSpeed()
        {
            if (timer == null) return;
            double framesPerSeconds = Math.Max((Const_OriginalHz * this.SpeedTimes), 1);
            double timeBetweenFrames = 1000 / framesPerSeconds;
            this.timer.Interval = timeBetweenFrames;
        }
        public void Play() {
            if (timer == null || fgAdapter == null || ts == null)
            {
                timer = new Timer();
                timer.Elapsed += delegate (object sender, ElapsedEventArgs e) {
                    this.CurrentTimeStep++; };

                fgAdapter = new FgAdapter(new FileDetails(this.FG_Path).OnlyPath);
                fgAdapter.Start(new FileDetails(this.XML_Path).NameWithoutExtension);
                this.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == "CurrentTimeStep")
                    this.fgAdapter?.SendPlayback(ts.getRowAsString(CurrentTimeStep));
                };
                fgAdapter.OnFGClosing += delegate () { this.IsPaused = true;
                                                       this.fgAdapter = null;
                                                    };
                ts = new TableSeries(this.TestCsv_Path, Utils.ParseFeaturesLine(this.XML_Path));
                NotifyPropertyChanged("MaxTimeStep", "AllFeaturesList");
            }
            UpdateSpeed();
            timer.Start();
        }

        

        public void Pause() {
            timer?.Stop();
        }
        public void CloseFG()
        {
            this.fgAdapter?.Close(true);
        }
    }
}
