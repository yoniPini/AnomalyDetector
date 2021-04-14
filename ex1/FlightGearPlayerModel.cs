using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Timers;

namespace ex1
{
    // class of player on time steps playback.
    // which we can subsribed to PropertyChanged to get notice when CurrentTimeStep changed
    // or we can change it. ("skip" time in video)

    // this is the center of the program
    public class FlightGearPlayerModel : IFlightGearPlayerModel
    {
        // ITableSeriesNotify :
        public Dictionary<string, float> FeaturesValue { get {
                var d = new Dictionary<string, float>();
                foreach (string s in AllFeaturesList)
                    if (!d.ContainsKey(s))
                        d.Add(s, Table.getCell(CurrentTimeStep, s));
                return d;
            } }
        
        public List<string> AllFeaturesList { get {
                return Table?.featuresStrAsList ?? new List<string>();
            } }

        // IFlightGearPlayerModel :
        public double Const_OriginalHz { get { return 10.00; } }


        // changes should be only from associated FlightGearPlayerViewModel [vm -> model] :
        public string FG_Path { get; set; }
        public string XML_Path { get; set; }
        public string LearnCsv_Path { get; set; }
        public string TestCsv_Path { get; set; }

        public bool IsPowerOn => (this.fgAdapter != null);

        private bool isRunningNow;
        public bool IsRunning { get { return isRunningNow; } set {
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
                NotifyPropertyChanged("CurrentTimeStep", "CurrentTimeInStr", "FeaturesValue"); } }
        
        
        // The return value is one of the aviable timesteps
        public int MaxTimeStep { get { 
                if(ts != null)
                    return ts.RowsLength - 1;
                return 0;
            } } 
        
        // get int between 0 and 99 and return its 2 digit representaion
        private String TwoDigitsRepr(int x)
        {
            if (x == 0) return "00";
            if (x >= 10) return "" + x;
            return "0" + x;
        }
        
        // "00:01:35 for example"
        public String CurrentTimeInStr { get {
                int curr = CurrentTimeStep;
                int currSec = curr / (int)Const_OriginalHz;
                int currMin = currSec / 60;
                int currHour = currMin / 60;
                currSec = currSec % 60;
                currMin = currMin % 60;
                return TwoDigitsRepr(currHour) + ":" + TwoDigitsRepr(currMin) + ":" + TwoDigitsRepr(currSec);
            } } 
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
        private bool shouldWaitForFgAdapter = true;
        
        
        // if there is no yet simulator, open one and establish connection (in diffrent thread).
        // start the playing = timestep increasment in timer
        public void Play() {
            if (timer == null || fgAdapter == null || ts == null)
            {
                timer = new Timer();
                timer.Elapsed += delegate (object sender, ElapsedEventArgs e) {
                    this.CurrentTimeStep++; };
                fgAdapter = new FgAdapter(new FileDetails(this.FG_Path).OnlyPath);

                this.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == "CurrentTimeStep" && ts != null)
                    this.fgAdapter?.SendPlayback(ts.getRowAsString(CurrentTimeStep));
                };
                fgAdapter.OnFGClosing += delegate () { 
                                                       // DONT ADD IsPaused = true
                                                       this.fgAdapter = null;
                                                       CloseFG();
                                                    };
                ts = new TableSeries(this.TestCsv_Path, Utils.ParseFeaturesLine(this.XML_Path));

                // wait to the simulator to be ready,
                // but in diffrent thread (the waiting, the simulator is in different process)
                this.shouldWaitForFgAdapter = true;
                new System.Threading.Thread(delegate ()
                {
                    fgAdapter?.Start(new FileDetails(this.XML_Path).NameWithoutExtension); // [code waiting] in this new thread

                    // DONT add IsPaused or IsRunning, to avoid infinty loop / problems with multithreads
                    if (timer == null || fgAdapter == null || ts == null)
                    {
                        currTimeStep = 0;
                        timer?.Stop();
                    } else
                    {
                        this.shouldWaitForFgAdapter = false;
                        UpdateSpeed();
                        timer?.Start();
                    }
                }).Start();
                NotifyPropertyChanged("MaxTimeStep", "AllFeaturesList");
                NotifyPropertyChanged("IsPowerOn");
            }
            // DONT add IsPaused or IsRunning, to avoid infinty loop / problems with multithreads
            if (shouldWaitForFgAdapter) return;
            UpdateSpeed();
            timer?.Start();
        }

        

        public void Pause() {
            timer?.Stop();
        }
        public void CloseFG()
        {
            this.fgAdapter?.Close(true);
            this.IsPaused = true;
            this.fgAdapter = null;
            NotifyPropertyChanged("IsPowerOn");
        }
    }
}
