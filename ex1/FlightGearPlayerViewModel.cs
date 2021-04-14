using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ex1
{
    // class which is between the view and the IFlightGearPlayerModel,
    // to give data to the view about current state of the playing(timestep, for example)
    // and to give the IFlightGearPlayerModel commands / data such as VM_FG_Path (bound to textbox in view)
    public class FlightGearPlayerViewModel : IFlightGearPlayerViewModel
    {
        public double Const_OriginalHz { get { return model.Const_OriginalHz; } }
        public string VM_FG_Path { get { return model.FG_Path; } set { model.FG_Path = value; } } 
        public string VM_XML_Path { get { return model.XML_Path; } set { model.XML_Path = value; } }
        public string VM_LearnCsv_Path { get { return model.LearnCsv_Path; } set { model.LearnCsv_Path = value; } }
        public string VM_TestCsv_Path { get { return model.TestCsv_Path; } set { model.TestCsv_Path = value; } }
        public bool VM_IsPowerOn => model.IsPowerOn; 
        public bool VM_IsPowerOff => !model.IsPowerOn; 
        public bool VM_IsRunning { 
            get { return model.IsRunning; }
            set {
                if (value == VM_IsRunning) return ;
                if (value == true)
                    Play();                 // do checks and not only model.IsRunning = true;
                else
                    model.IsRunning = false;
                    } 
        }
        public bool VM_IsPaused { get { return !VM_IsRunning; } set { VM_IsRunning = !value; } } // opposite of the above

        public int VM_CurrentTimeStep { get { return model.CurrentTimeStep; } set { model.CurrentTimeStep = value; } }
        public int VM_MaxTimeStep { get { return model.MaxTimeStep; } }
        public String VM_CurrentTimeInStr { get { return model.CurrentTimeInStr; } }
        public double VM_SpeedTimes
        {
            get { return model.SpeedTimes; }
            set { model.SpeedTimes = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private IFlightGearPlayerModel model;
        public FlightGearPlayerViewModel(IFlightGearPlayerModel model) {
            this.model = model;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged("VM_" + e.PropertyName);
                if (e.PropertyName == "IsPowerOn") NotifyPropertyChanged("VM_IsPowerOff");
            };
        }
        private void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
                this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
        
        // before active IsRunning of the IFlightGearPlayer check the 4 paths are valid and correct
        public void Play()
        {
            // check 4 path are exist
            // (if exist then they valid because we got them from dialogOpen in view[using Utils class])
            if (this.VM_FG_Path == "" || this.VM_XML_Path == "" ||
                        this.VM_LearnCsv_Path == "" || this.VM_TestCsv_Path == "")
            {
                System.Windows.MessageBox.Show("Please fill all the 4 paths below.");
                return;
            }
            // check that there is correspond xml record protocol IN THE data\Protocol folder of flight gear
            // (assuming VM_FG_Path is correct)
            var fgMainFolder = new FileDetails(VM_FG_Path).OnlyPath + "..\\";
            var protocolsFolder = fgMainFolder + @"data\Protocol\";
            var xml = new FileDetails(VM_XML_Path);
            if (!System.IO.File.Exists(protocolsFolder + xml.OnlyFullName))
            {
                System.Windows.MessageBox.Show("Please Copy the xml file to the fg protocol folder.", "Alert");
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", '\"' + xml.OnlyPath + '\"');
                    System.Diagnostics.Process.Start("explorer.exe", '\"' + protocolsFolder + '\"');
                }
                catch { }
                return;
            }
            // if all the checks succeded, play the model
            this.model.IsRunning = true;
        }
        public void CloseFG()
        {
            model.CloseFG();
        }
    }
}
