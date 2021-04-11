using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.ComponentModel;

namespace ex1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // assuming throttleSlider value is [0,1]
        // assuming rudderSlider value is [-1, 1]
        private string f1 = "Feature1Traces";
        private string f2 = "Feature2Traces";
        private string f1Andf2 = "Features1And2";
        public PlotModel PlotModelF1 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public IFlightGearPlayerViewModel FG_Player_VM { get; }
        public IJoystickViewModel Joystick_VM { get; }
        public ITableSeriesNotify TableListener { get; }
        public IAnomalySelectViewModel AnomalySelect_VM { get; }
        public IOxyViewModel OxyViewModel_VM_F1 { get; }
        public IOxyViewModel OxyViewModel_VM_F2 { get; }
        public IOxyViewModel OxyViewModel_VM_F1AndF2 { get; }
        public MainWindow()
        {
            var fgModel = new FlightGearPlayerModel();
            var anomalyGraphModel = new AnomalyGraphModel(fgModel);

            this.FG_Player_VM = new FlightGearPlayerViewModel(fgModel);
            this.Joystick_VM = new JoystickViewModel(new JoystickModel(fgModel));
            this.TableListener = fgModel;
            this.AnomalySelect_VM = new AnomalySelectViewModel(anomalyGraphModel);
            this.OxyViewModel_VM_F1 = new OxyViewModel(anomalyGraphModel, f1);
            this.OxyViewModel_VM_F2 = new OxyViewModel(anomalyGraphModel, f2);
            this.OxyViewModel_VM_F1AndF2 = new OxyViewModel(anomalyGraphModel, f1Andf2);
            SetUpFeature1Graph();
            SetUpFeature2Graph();
            SetUpFeature1AndF2Graph();

            InitializeComponent();
            DataContext = this;
            Detectors_ComboBox.Items.Add(new ComboBoxItem()
            {
                Content = "- Not Selected -",
                ToolTip = AnomalyDetectorsManager.EmptyAnomalyDetector.Name + "\n" + AnomalyDetectorsManager.EmptyAnomalyDetector.Description});
            Detectors_ComboBox.SelectedIndex = 0;
            /*
             to add 3  IOxyViewModel and to subscribe to each PropertyChanged
             */
            AnomalySelect_VM.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName != "VM_AllFeaturesList") return;
                List<string> list = AnomalySelect_VM.VM_AllFeaturesList;
                list.Sort();
                FeaturesListBox.ItemsSource = list;
                CollectionView cv = CollectionViewSource.GetDefaultView(FeaturesListBox.ItemsSource) as CollectionView;
                cv.Filter = new Predicate<object>(  delegate (object o) {
                    string s = o as string;
                    return s?.ToUpper()?.Contains(FilterFeatures_TextBox.Text?.ToUpper()?.Trim(' ', '\t') ?? "") ?? false;
                    }   );
                FeaturesListBox.Items.IsLiveFiltering = true;
                FilterFeatures_TextBox.TextChanged += delegate (object t, TextChangedEventArgs e2) {
                    cv.Refresh();
                    };
            };

            this.Closed += delegate (object sender, EventArgs e) { FG_Player_VM.CloseFG(); };
        }
        
        private void SetUpFeature1Graph()
        {
            PlotModelF1 = new PlotModel();
            PlotModelF1.Axes.Add(OxyViewModel_VM_F1.LeftAxis);
            PlotModelF1.Axes.Add(OxyViewModel_VM_F1.BottomAxis);
            OxyViewModel_VM_F1.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == f1)
                    UpdateFeatures1Graph();
            };
        }
        private void UpdateFeatures1Graph()
        {
            Series ls = OxyViewModel_VM_F1.Ls;
            PlotModelF1.Series.Remove(ls);
            if (ls != null)
                PlotModelF1.Series.Add(ls);
        }
        
        private void SetUpFeature2Graph()
        {
            OxyViewModel_VM_F2.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == f2)
                    UpdateFeatures2Graph();
            };
        }
        private void UpdateFeatures2Graph()
        {

        }
        
        private void SetUpFeature1AndF2Graph()
        {
            OxyViewModel_VM_F1AndF2.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == f1Andf2)
                    UpdateFeatures1AndF2Graph();
            };
        }
        private void UpdateFeatures1AndF2Graph()
        {

        }

        private void SpeedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            KeyConverter kc = new KeyConverter();
            if (e.Key.ToString() != "Return")
                return;
            MessageBox.Show(kc.ConvertToString(e.Key),e.Key.ToString());
        }

        private void Minus10SecButton_Click(object sender, RoutedEventArgs e)
        {
            this.FG_Player_VM.VM_CurrentTimeStep -= (int)(10 * FG_Player_VM.Const_OriginalHz);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.FG_Player_VM.VM_IsRunning = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.FG_Player_VM.VM_IsPaused = true;
            this.FG_Player_VM.VM_CurrentTimeStep = 0;
            this.FG_Player_VM.CloseFG();
            this.Detectors_ComboBox.SelectedIndex = 0;
        }

        private void Plus10SecButton_Click(object sender, RoutedEventArgs e)
        {
            this.FG_Player_VM.VM_CurrentTimeStep += (int)(10 * FG_Player_VM.Const_OriginalHz);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            this.FG_Player_VM.VM_IsPaused = true;
        }

        private void FG_Path_Button_Click(object sender, RoutedEventArgs e)
        {
            //this.FG_Path_TextBox.TextChanged += delegate (object x, TextChangedEventArgs xx) { this.FG_Player_VM.test(); };
            this.FG_Path_TextBox.Text =
                Utils.GetFilePathFromUserGUI("Flight Gear Simulator", "fgfs.exe");
        }

        private void XML_Path_Button_Click(object sender, RoutedEventArgs e)
        {
            this.XML_Path_TextBox.Text =
                Utils.GetFilePathFromUserGUI("Flight Protocol file", "*.xml");
        }

        private void LearnCsv_Path_Button_Click(object sender, RoutedEventArgs e)
        {
            this.LearnCsv_Path_TextBox.Text = 
                Utils.GetFilePathFromUserGUI("Normal Flight recording file", "*.csv;*.txt;*.log");
        }

        private void TestCsv_Path_Button_Click(object sender, RoutedEventArgs e)
        {
            this.TestCsv_Path_TextBox.Text = 
                Utils.GetFilePathFromUserGUI("Flight recording file", "*.csv;*.txt;*.log");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isYoni = false;
            if (isYoni)
            {
                this.FG_Path_TextBox.Text = @"C:\Program Files\FlightGear 2020.3.6\bin\fgfs.exe";
                this.LearnCsv_Path_TextBox.Text = @"C:\Users\WIN10\OneDrive\שולחן העבודה\מסמכים של בר אילן\שנה ב\‏‏סמסטר ב\תכנות מתקדם 2\קבצים לפרויקט-20210324\reg_flight.csv";
                this.TestCsv_Path_TextBox.Text = @"C:\Users\WIN10\OneDrive\שולחן העבודה\מסמכים של בר אילן\שנה ב\‏‏סמסטר ב\תכנות מתקדם 2\קבצים לפרויקט-20210324\reg_flight.csv";
                this.XML_Path_TextBox.Text = @"C:\Program Files\FlightGear 2020.3.6\data\Protocol\playback_small.xml";
            }
            else
            {
                //this is only to be fast:
                this.FG_Path_TextBox.Text = @"C:\Program Files\FlightGear_2020.3.6\bin\fgfs.exe";
                this.LearnCsv_Path_TextBox.Text = @"C:\Users\EhudV\Desktop\ap2_ex1\reg_flight.csv";
                this.TestCsv_Path_TextBox.Text = @"C:\Users\EhudV\Desktop\ap2_ex1\reg_flight.csv";
                this.XML_Path_TextBox.Text = @"C:\Users\EhudV\Desktop\ap2_ex1\playback_small.xml";
            }
            //MessageBox.Show(this.FG_Player_VM.VM_SpeedTimes.ToString());
            //MessageBox.Show(this.Joystick_SmallCircle.ActualHeight.ToString());
            //var x = this.Joystick_SmallCircle.Margin;
            //this.Joystick_SmallCircle.Margin = new Thickness(x.Left-15,x.Top, 0, 0);
            // this.Joystick_SmallCircle.TransformToAncestor(this).Transform(new Point(9,-9));
        }

        private void AddDetector_Click(object sender, RoutedEventArgs e)
        {
            var path = Utils.GetFilePathFromUserGUI("Anomaly Detecion algorithim", "*.dll");

            if (String.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path)) { 
                MessageBox.Show("No dll was choosen.");
                return;
            }
            var s = this.AnomalySelect_VM.LoadDetectorFromDll(path);
            if (String.IsNullOrWhiteSpace(s))
            {
                MessageBox.Show("Unable to load the dll, or read the csv files.");
            }
           // Detectors_ComboBox.Items.Add(.NameWithoutExtension);
            Detectors_ComboBox.Items.Add(new ComboBoxItem() { 
                Content = new FileDetails(path).OnlyFullName,
                ToolTip = s + "\n\nLoaded from :\n" + path});

            //  |w| < p(|<M, x>|)

            //  (<M', x'>, 1) not in R
        }
    }
}
