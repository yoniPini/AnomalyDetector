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
        private LineSeries ls;
        private ScatterSeries normal;
        private ScatterSeries aNormal;
        private LinearAxis leftAxis = new LinearAxis()
        {
            Position = AxisPosition.Left,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            Title = "",
            StartPosition = 0,
        };

        private LinearAxis bottomAxis = new LinearAxis()
        {
            Position = AxisPosition.Bottom,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            Title = "",
            StartPosition = 0,
        };

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
            get { return bottomAxis; }
        }

        public LinearAxis LeftAxis
        {
            get { return leftAxis; }
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
            //ls.Points.Add(new DataPoint(5, 6)); // debug
            
            var list = anomalyGraphModel.Feature1Traces;
            ls.Points.Clear();
            foreach (var item in list)
                ls.Points.Add(new DataPoint(item.x, item.y));
        }

        private void feature2Update()
        {
            var list = anomalyGraphModel.Feature2Traces;
            ls.Points.Clear();
            foreach (var item in list)
                ls.Points.Add(new DataPoint(item.x, item.y));
        }

        private void bothFeatures1Update()
        {
            var list = anomalyGraphModel.Features1And2;
            normal.Points.Clear();
            aNormal.Points.Clear();
            foreach (var item in list) {
                if (!item.isAnomaly)
                    normal.Points.Add(new ScatterPoint(item.x, item.y));
                else
                    aNormal.Points.Add(new ScatterPoint(item.x, item.y));
            }
        }


        public OxyViewModel(IAnomalyGraphModel a, string p)
        {
            
            ls = new LineSeries();
            ls.Color = OxyColors.Blue;
            //thickness
            normal = new ScatterSeries();
            normal.MarkerFill = OxyColors.Blue;
            normal.MarkerStrokeThickness = 0.05;
            normal.MarkerType = MarkerType.Circle;

            aNormal = new ScatterSeries();
            aNormal.MarkerFill = OxyColors.Red;
            aNormal.MarkerStrokeThickness = 0.05;
            aNormal.MarkerType = MarkerType.Circle;

            anomalyGraphModel = a;
            property = p;
            anomalyGraphModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (property == e.PropertyName)
                {
                    setCorrectOption();
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
                }
            };
        }

    }
}
