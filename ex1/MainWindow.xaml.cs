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

namespace ex1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void SpeedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            KeyConverter kc = new KeyConverter();
            if (e.Key.ToString() != "Return")
                return;
            MessageBox.Show(kc.ConvertToString(e.Key),e.Key.ToString());
        }

        private void OpenTestButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenTrainButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                MessageBox.Show(openFileDialog.FileName);
        }

        private void Minus10SecButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Plus10SecButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
