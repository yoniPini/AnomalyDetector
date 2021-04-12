using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
//using System.Timers;
namespace ex1
{
    public class AnomalySelectViewModel : IAnomalySelectViewModel
    {
        private IAnomalyGraphModel model;
        public AnomalySelectViewModel(IAnomalyGraphModel model)
        {
            this.model = model;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }
        private void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
                this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
        private readonly List<string> featuresListConst = new List<string>();
        public List<string> VM_AllFeaturesList { get {
                featuresListConst.Clear();
                if (model.AllFeaturesList != null)
                    featuresListConst.AddRange(model.AllFeaturesList);
                return featuresListConst; 
            } }
        public int VM_SelectedDetectorIdx
        {
            get { return model.SelectedDetectorIdx; }
            set
            {
                model.SelectedDetectorIdx = value;
                if (!model.Analyze() && value != 0)
                {
                    System.Windows.MessageBox.Show("Unable to read the csv files.");
                    model.SelectedDetectorIdx = 0;
                }
            }
        }         //x.DefaultFeatures; x.Detect; x.Learn;
        public string LoadDetectorFromDll(string fromDllPath)
        {
            return model.LoadDetectorFromDll(fromDllPath);
        }
        public bool Analyze() { return model.Analyze(); }
        public string VM_Feature1
        {
            get { return model.Feature1; }
            set { model.Feature1 = value; }
        } // i.e. SelectedFeature
        public bool VM_IsFeature2Exists { get { return model.IsFeature2Exists; } }
        public string VM_Feature2 { get { return model.Feature2; } }
        public string VM_CorrelationObjectType { get { return model.CorrelationObjectType; } }

        // INotifyPropertyChanged:
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
