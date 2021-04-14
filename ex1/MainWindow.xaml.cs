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
        public PlotModel PlotModelF2 { get; set; }
        public PlotModel PlotModelF1AndF2 { get; set; }
        //axis  is zoom enabled
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
            ToolTipStr = new Dictionary<string, string>()
            {
                {"F1", null },
                {"F2", null },
                {"F1AndF2", null },
            };

            InitializeComponent();
            
            SetUpFeature1Graph();
            SetUpFeature2Graph();
            SetUpFeature1AndF2Graph();

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

            FilterFeatures_TextBox.LostFocus += delegate (object s, RoutedEventArgs e)
                {
                    var x = FilterFeatures_TextBox.Text?.Trim(' ', '\t') ?? "";
                    if (x == "")
                    {
                        FilterFeatures_TextBox.Text = "";
                        FilterHintLabel.Visibility = Visibility.Visible;
                    }
                    //else
                    //{
                    //    FilterHintLabel.Visibility = Visibility.Hidden;
                    //}
                };
            FilterFeatures_TextBox.GotFocus += delegate (object s, RoutedEventArgs e)
            {
                FilterHintLabel.Visibility = Visibility.Hidden;
            };
            this.Closed += delegate (object sender, EventArgs e) { FG_Player_VM.CloseFG(); };
            Feature1And2Graph.MouseLeave += delegate (object s, MouseEventArgs e)
            {
                PlotModelF1AndF2.ResetAllAxes();
                Feature1And2Graph.InvalidatePlot();
            };
            
            Feature1Graph.MouseLeave += delegate (object s, MouseEventArgs e)
            {
                PlotModelF1.ResetAllAxes();
                Feature1Graph.InvalidatePlot();
            };
            
            Feature2Graph.MouseLeave += delegate (object s, MouseEventArgs e)
            {
                PlotModelF2.ResetAllAxes();
                Feature2Graph.InvalidatePlot();
            };
        }
        
        private string AdjustLegendTitle(string s)
        {
            if (s?.Length > 14)
                return s.Substring(0, 14) + "...";
            return s;
        }

        private void SetUpLegendProperties(PlotModel p)
        {
            p.LegendOrientation = LegendOrientation.Horizontal;
            p.LegendPlacement = LegendPlacement.Outside;
            p.LegendPosition = LegendPosition.TopRight;
            p.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            p.LegendBorder = OxyColors.Black;
            p.LegendTitleFontWeight = 0;
            p.LegendMaxHeight = 30;
            p.LegendMaxWidth = 100;
            p.LegendMargin = 5;
            p.LegendSymbolMargin = 5;
        }

        private void SetUpFeature1Graph()
        {
            PlotModelF1 = new PlotModel();
            SetUpLegendProperties(PlotModelF1);
            PlotModelF1.Axes.Add(OxyViewModel_VM_F1.LeftAxis);
            PlotModelF1.Axes.Add(OxyViewModel_VM_F1.BottomAxis);
            OxyViewModel_VM_F1.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == f1)
                    UpdateFeatures1Graph();
            };
        }

        public Dictionary<string, string> ToolTipStr{ get; }


        private void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
                this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }

        private void UpdateFeatures1Graph()
        {
            Series ls = OxyViewModel_VM_F1.Ls;
            if (ls != null)
            {
                if (ToolTipStr["F1"] != OxyViewModel_VM_F1.Legend)
                {
                    ToolTipStr["F1"] = OxyViewModel_VM_F1.Legend;
                    NotifyPropertyChanged("ToolTipStr");
                }
                 //   Feature1Graph.ToolTip = OxyViewModel_VM_F1.Legend;
                PlotModelF1.LegendTitle = AdjustLegendTitle(OxyViewModel_VM_F1.Legend);
                PlotModelF1.Series.Remove(ls);
                PlotModelF1.Series.Add(ls);
                Feature1Graph.InvalidatePlot(true);
            }
        }

        private void SetUpFeature2Graph()
        {
            PlotModelF2 = new PlotModel();
            SetUpLegendProperties(PlotModelF2);
            PlotModelF2.Axes.Add(OxyViewModel_VM_F2.LeftAxis);
            PlotModelF2.Axes.Add(OxyViewModel_VM_F2.BottomAxis);
            
            OxyViewModel_VM_F2.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == f2)
                    UpdateFeatures2Graph();
            };
        }
        private void UpdateFeatures2Graph()
        {
            Series ls = OxyViewModel_VM_F2.Ls;
            if (ls != null)
            {
                if (OxyViewModel_VM_F2.IsFeature2Exists)
                {
                    PlotModelF2.LegendTitle = AdjustLegendTitle(OxyViewModel_VM_F2.Legend);
                    if (ToolTipStr["F2"] != OxyViewModel_VM_F2.Legend)
                    {
                        ToolTipStr["F2"] = OxyViewModel_VM_F2.Legend;
                        NotifyPropertyChanged("ToolTipStr");
                    }
                }
                else
                {
                    PlotModelF2.LegendTitle = "cor undetected";
                    if (ToolTipStr["F2"] != "no correlation")
                    {
                        ToolTipStr["F2"] = "no correlation";
                        NotifyPropertyChanged("ToolTipStr");
                    }
                }
                //PlotModelF2.TitleToolTip = OxyViewModel_VM_F2.Legend;
                PlotModelF2.Series.Remove(ls);
                PlotModelF2.Series.Add(ls);
                Feature2Graph.InvalidatePlot(true);
            }
        }
        
        private void SetUpFeature1AndF2Graph()
        {
            PlotModelF1AndF2 = new PlotModel();
            SetUpLegendProperties(PlotModelF1AndF2);
            PlotModelF1AndF2.Axes.Add(OxyViewModel_VM_F1AndF2.LeftAxis);
            PlotModelF1AndF2.Axes.Add(OxyViewModel_VM_F1AndF2.BottomAxis);
            OxyViewModel_VM_F1AndF2.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == f1Andf2)
                    UpdateFeatures1AndF2Graph();
            };
        }
        private void UpdateFeatures1AndF2Graph()
        {
            Series ls = OxyViewModel_VM_F1AndF2.Normal;
            Series ls2 = OxyViewModel_VM_F1AndF2.ANormal;
            Series ls3 = OxyViewModel_VM_F1AndF2.CorrelationObject;
            if (ls != null && ls2 != null)
            {
                if (ToolTipStr["F1AndF2"] != OxyViewModel_VM_F1AndF2.Legend && OxyViewModel_VM_F1AndF2.IsFeature2Exists)
                {
                    var s = OxyViewModel_VM_F1AndF2.Legend.Split('\n');
                    
                    ToolTipStr["F1AndF2"] ="X: " + s[0] + "\n" +"Y: " + s[1];
                    NotifyPropertyChanged("ToolTipStr");
                }
                else if (!OxyViewModel_VM_F1AndF2.IsFeature2Exists && ToolTipStr["F1AndF2"] != "no correlation")
                {
                    ToolTipStr["F1AndF2"] = "no correlation";
                    NotifyPropertyChanged("ToolTipStr");
                }

                

                //PlotModelF1AndF2.TitleToolTip = "x: " + OxyViewModel_VM_F1.Legend + "\ny: " + OxyViewModel_VM_F2.Legend;
                PlotModelF1AndF2.Series.Remove(ls);
                PlotModelF1AndF2.Series.Remove(ls2);
                PlotModelF1AndF2.Series.Remove(ls3);
                PlotModelF1AndF2.Series.Add(ls);
                PlotModelF1AndF2.Series.Add(ls2);
                if (ShowCorObject_CheckBoxIsChecked == true)
                    PlotModelF1AndF2.Series.Add(ls3);
                Feature1And2Graph.InvalidatePlot(true);
            }
        }
        public bool ShowCorObject_CheckBoxIsChecked { get; set; }
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

        private void AddDetector_Click(object sender, RoutedEventArgs e)
        {
            var path = Utils.GetFilePathFromUserGUI("Anomaly Detecion algorithim", "*.dll");

            if (String.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                MessageBox.Show("No dll was choosen.");
                return;
            }
            var s = this.AnomalySelect_VM.LoadDetectorFromDll(path);
            if (String.IsNullOrWhiteSpace(s))
            {
                MessageBox.Show("Unable to load the dll.");
                return;
            }
            Detectors_ComboBox.Items.Add(new ComboBoxItem()
            {
                Content = new FileDetails(path).OnlyFullName,
                ToolTip = s + "\n\nLoaded from :\n" + path
            });

            AddedDetectorLabelsVisibility = Visibility.Visible;
            new System.Threading.Thread(delegate () {
                System.Threading.Thread.Sleep(5000);
                AddedDetectorLabelsVisibility = Visibility.Hidden;
            }).Start();
        }
        private Visibility m_AddedDetectorLabelsVisibility = Visibility.Hidden;
        public Visibility AddedDetectorLabelsVisibility
        {
            get { return m_AddedDetectorLabelsVisibility; }

            set
            {
                m_AddedDetectorLabelsVisibility = value;
                NotifyPropertyChanged("AddedDetectorLabelsVisibility");
            }
        }

        public IFlightGearPlayerViewModel IFlightGearPlayerViewModel
        {
            get => default;
            set
            {
            }
        }

        public IFlightGearPlayerViewModel IFlightGearPlayerViewModel1
        {
            get => default;
            set
            {
            }
        }

        public IJoystickViewModel IJoystickViewModel
        {
            get => default;
            set
            {
            }
        }

        public IAnomalyDetectorsManager IAnomalyDetectorsManager
        {
            get => default;
            set
            {
            }
        }

        public IOxyViewModel IOxyViewModel
        {
            get => default;
            set
            {
            }
        }
    }
}
