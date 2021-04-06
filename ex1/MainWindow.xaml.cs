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
        public event PropertyChangedEventHandler PropertyChanged;
        public IFlightGearPlayerViewModel FG_Player_VM { get; }
        public MainWindow()
        {
            this.FG_Player_VM = new FlightGearPlayerViewModel(new FlightGearPlayerModel());
            InitializeComponent();
            DataContext = this;

            



            //AnomalyDetectorsManager y = new AnomalyDetectorsManager();
            //DLL.IAnomalyDetector x = y.AddAnomalyDetector(Utils.GetFileDetailsFromUserGUI(
               // "anomaly file", "*.dll").FullPath);
            //MessageBox.Show(Utils.ParseFeaturesLine(Utils.GetFileDetailsFromUserGUI(
             //   "protocol file", "*.xml").FullPath));
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

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.FG_Player_VM.VM_IsRunning = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Plus10SecButton_Click(object sender, RoutedEventArgs e)
        {
            //this is only to be fast:
            this.FG_Path_TextBox.Text = @"C:\Program Files\FlightGear_2020.3.6\bin\fgfs.exe";
            this.LearnCsv_Path_TextBox.Text = @"C:\Users\EhudV\Desktop\ap2_ex1\reg_flight.csv";
            this.TestCsv_Path_TextBox.Text = @"C:\Users\EhudV\Desktop\ap2_ex1\reg_flight.csv";
            this.XML_Path_TextBox.Text = @"C:\Users\EhudV\Desktop\ap2_ex1\playback_small.xml";
            MessageBox.Show(this.FG_Player_VM.VM_SpeedTimes.ToString());
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {

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

    }
}
