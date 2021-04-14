/**
 * every class who implements this interface behaves differently according the parameters given in the constructor.
 * this interface ensures to have properties to draw graphs, i.e. axis line/scatter series and a legend.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL;
using System.ComponentModel;

using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;

namespace ex1
{
    public interface IOxyViewModel : INotifyPropertyChanged
    {
        // model: graphModel: SpecialPoint -> 
        // view-model: IOxyViewModel: LineSeries ->
        // view :MainWindow -> SetUp(oxyViewModel)
        LineSeries Ls { get; } // relevant for  feature1Traces, and also for feature2Traces.
        //in the IMPLEMENTATION : private IAnomalyGraphModel graphModel;
        ScatterSeries Normal { get; } //relevant for  feature1And2.
        ScatterSeries ANormal { get; } //relevant for  feature1And2.
        LinearAxis BottomAxis { get; }
        LinearAxis LeftAxis { get; }

        string Legend { get; } 
        bool IsFeature2Exists { get; }
        Series CorrelationObject { get; }
    }
}