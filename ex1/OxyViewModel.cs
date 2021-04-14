/**
 * this class is responsible for supllying the relevant data according the given string property.
 * for example' when given "Feature1Traces", this class will keep listening only to the events
 * that relevant to the first feature to keep the grapg updated .
 * As said before this class listen to another class which represent a dort of model as the MVVM
 * structure tells.
 */
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
        private LineSeries objectCor;
        /**
         * the axes are the same for all Graphs.
         */
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

        /*
         * this method pulls the relevant data from the modelClass befores updating the listeners of a changed prorerty. */
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

        /*
         * set the objectCor field with the correct shape to draw, a line or ta circle. in case of an other shapes
         * the person who gives the DLL file should implement the IOxyPlotDrawable interface to get from him a llist of points
         * to draw lines between them. under that manner rounded shapes are NOT supported.
         */
        private void UpdateObjectCor()
        {
            objectCor.Points.Clear();
            var c = anomalyGraphModel.CorrelationObject as DLL.Circle;
            if (c != null)
            {
                Func<double, double> FirstHalfcircle = (x) => CenterCircleYPlus(c.x, c.y, x, c.r);
                Func<double, double> SecondtHalfcircle = (x) => CenterCircleYMinus(c.x, c.y, x, c.r);
                var list = new FunctionSeries(FirstHalfcircle, c.x - c.r, c.x + c.r, c.r / 100);
                objectCor.Points.AddRange(list.Points);
                var list2 = new FunctionSeries(SecondtHalfcircle, c.x - c.r, c.x + c.r, c.r /100);
                objectCor.Points.AddRange(list2.Points);
                return;
            }
            var l = anomalyGraphModel.CorrelationObject as DLL.Line;
            if (l != null)
            {
                float maxX = float.NegativeInfinity;
                float minX = float.PositiveInfinity;
                foreach (var sp in anomalyGraphModel.Features1And2) {
                    maxX = Math.Max(sp.x, maxX);
                    minX = Math.Min(sp.x, minX);
                }
                float dist = maxX - minX;
                minX -= dist * 0.7f;
                maxX += dist * 0.7f;
                objectCor.Points.Add(new DataPoint(minX, l.f(minX)));
                objectCor.Points.Add(new DataPoint(maxX, l.f(maxX)));
                return;
            }

            var o = anomalyGraphModel.CorrelationObject as DLL.IOxyPlotDrawable;
            var listPoints = o?.GetShape();
            if (listPoints != null)
                objectCor.Points.AddRange(listPoints);
        }

        private void feature1Update()
        {
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
               
        /*
         * the constructor gets a string p that changes the behavior of the instance accordingly (further details in setup method).
         * It also initialize the fields with the same values (exept property) since it dont matter from one instance to another.
         */
        public OxyViewModel(IAnomalyGraphModel a, string p)
        {
            ls = new LineSeries();
            ls.Color = OxyColors.Blue;
            //setCorrectLegend();
            //thickness
            normal = new ScatterSeries();
            normal.MarkerFill = OxyColors.Blue;
            normal.MarkerStrokeThickness = 0.005;
            normal.MarkerType = MarkerType.Circle;

            aNormal = new ScatterSeries();
            aNormal.MarkerFill = OxyColors.Red;
            aNormal.MarkerStrokeThickness = 0.005;
            aNormal.MarkerType = MarkerType.Circle;

            objectCor = new LineSeries();
            objectCor.Color = OxyColors.Black;

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
