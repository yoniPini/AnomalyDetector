using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex1
{
    public class OxyViewModel : IOxyViewModel
    {
        private IAnomalyGraphModel anomalyGraphModel;
        string property;
        private LineSeries ls = null;
        private ScatterSeries normal = null;
        private ScatterSeries aNormal = null;

        public LineSeries Ls
        {
            get { return ls; }
        }

        public ScatterSeries Normal
        {
            get { return normal; }
        }
        public ScatterSeries ANormal
        {
            get { return aNormal; }
        }
        public LinearAxis BottomAxis
        {
            get { return null; }
        }

        public LinearAxis LeftAxis
        {
            get { return null; }
        }

        public string LegendTitle
        {
            get { return null; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void setCorrectOption()
        {
            switch (property)
            {
                case "Feature1Traces":
                    feature1Update();
                    break;
                case "Feature2Traces":
                    feature2Update();
                    break;
                case "Features1And2":
                    bothFeatures1Update();
                    break;
                default:
                    break;
            }
        }

        private void feature1Update()
        {
            var list = anomalyGraphModel.Feature1Traces;
            foreach (var item in list)
                ls.Points.Add(new DataPoint(item.x, item.y));
        }

        private void feature2Update()
        {
            var list = anomalyGraphModel.Feature2Traces;
            foreach (var item in list)
                ls.Points.Add(new DataPoint(item.x, item.y));
        }

        private void bothFeatures1Update()
        {
            var list = anomalyGraphModel.Features1And2;
            foreach (var item in list) {
                if (!item.isAnomaly)
                    normal.Points.Add(new ScatterPoint(item.x, item.y));
                else
                    aNormal.Points.Add(new ScatterPoint(item.x, item.y));
            }
        }


        OxyViewModel(IAnomalyGraphModel a, string p)
        {
            anomalyGraphModel = a;
            property = p;
            anomalyGraphModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                setCorrectOption();
                if (property == e.PropertyName)
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            };
        }

    }
}
