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
        string legend;
        private LineSeries ls;
        private ScatterSeries normal;
        private ScatterSeries aNormal;
        private FunctionSeries objectCor;
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


        public string Legend
        {
            get
            {
                return legend;
            }    
        }

        public bool IsFeature2Exists
        {
            get
            { return anomalyGraphModel.IsFeature2Exists; }
        }

        public Series CorrelationObject
        {
            get
            {
                return objectCor;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void setCorrectOption()
        {
            switch (property)
            {
                case "Feature1Traces":
                    legend = anomalyGraphModel.Feature1;
                    feature1Update();
                    break;
                case "Feature2Traces":
                    legend = anomalyGraphModel.Feature2;
                    feature2Update();
                    break;
                case "Features1And2":
                    legend = anomalyGraphModel.Feature1 + "\n" + anomalyGraphModel.Feature2;
                    UpdateObjectCor();
                    bothFeaturesUpdate();
                    break;
                default:
                    break;
            }
        }

        private static double CenterCircleYPlus(double x0, double y0, double x, double r)
        {
            return y0 + Math.Sqrt(Math.Pow(r, 2) - Math.Pow(x - x0, 2));
        }

        private static double CenterCircleYMinus(double x0, double y0, double x, double r)
        {
            return y0 - Math.Sqrt(Math.Pow(r, 2) - Math.Pow(x - x0, 2));
        }

        private void UpdateObjectCor()
        {
            var c = anomalyGraphModel.CorrelationObject as DLL.Circle;
            if (c != null)
            {
                Func<double, double> FirstHalfcircle = (x) => CenterCircleYPlus(c.x, c.y, x, c.r);
                Func<double, double> SecondtHalfcircle = (x) => CenterCircleYMinus(c.x, c.y, x, c.r);
            }
            var l = anomalyGraphModel.CorrelationObject as DLL.Line;
            if (l != null)
            {
                LineSeries line = new LineSeries();

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

        private void bothFeaturesUpdate()
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
            //setCorrectLegend();
            //thickness
            normal = new ScatterSeries();
            normal.MarkerFill = OxyColors.Blue;
            normal.MarkerStrokeThickness = 0.05;
            normal.MarkerType = MarkerType.Circle;

            aNormal = new ScatterSeries();
            aNormal.MarkerFill = OxyColors.Red;
            aNormal.MarkerStrokeThickness = 0.05;
            aNormal.MarkerType = MarkerType.Circle;

            objectCor = new FunctionSeries();

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
