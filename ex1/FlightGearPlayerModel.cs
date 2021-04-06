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
                if (value < 0 || (ts != null && value >= ts.RowsLength))
                {
                    this.IsPaused = true;
                    return;
                }
                currTimeStep = value; 
                NotifyPropertyChanged("CurrentTimeStep", "CurrentTimeInStr"); } } // 95 for example
        private String TwoDigitsRepr(int x)
        {
            if (x == 0) return "00";
            if (x >= 10) return "" + x;
            return "0" + x;
        }
        public String CurrentTimeInStr { get {
                int curr = CurrentTimeStep;
                int currSec = curr / 10;
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
            int framesPerSeconds = Math.Max((int)(10 * this.SpeedTimes), 1);
            int timeBetweenFrames = 1000 / framesPerSeconds;
            if (timer != null)
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
                fgAdapter.OnFGClosing += delegate () { this.IsPaused = true; this.fgAdapter.Close();
                                                       this.fgAdapter = null;
                                                    };
                ts = new TableSeries(this.TestCsv_Path, Utils.ParseFeaturesLine(this.XML_Path));
            }
            UpdateSpeed();
            timer.Start();
        }

        

        public void Pause() {
            timer?.Stop();
        }
    }
}
