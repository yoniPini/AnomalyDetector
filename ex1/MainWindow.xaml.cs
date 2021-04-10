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
        public event PropertyChangedEventHandler PropertyChanged;
        public IFlightGearPlayerViewModel FG_Player_VM { get; }
        public IJoystickViewModel Joystick_VM { get; }
        public ITableSeriesListener TableListener { get; }
        public IAnomalySelectViewModel AnomalySelect_VM { get; }
        public MainWindow()
        {
            var fgModel = new FlightGearPlayerModel();
            var anomalyGraphModel = new AnomalyGraphModel(fgModel);

            this.FG_Player_VM = new FlightGearPlayerViewModel(fgModel);
            this.Joystick_VM = new JoystickViewModel(new JoystickModel(fgModel));
            this.TableListener = fgModel;
            this.AnomalySelect_VM = new AnomalySelectViewModel(anomalyGraphModel);

            InitializeComponent();
            DataContext = this;

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



            this.Closed += delegate (object sender, EventArgs e) { fgModel.CloseFG(); };
            
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

            if (String.IsNullOrWhiteSpace(this.AnomalySelect_VM.LoadDetectorFromDll(path)))
            {
                MessageBox.Show("Unable to load the dll, or read the csv files.");
            }
            Detectors_ComboBox.Items.Add(new FileDetails(path).NameWithoutExtension);
        }
    }
}
