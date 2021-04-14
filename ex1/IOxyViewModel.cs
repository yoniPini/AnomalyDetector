/**
 * this class is responsible for supllying the relrvant data according the given string property.
 * for example' when given "Feature1Traces" 
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
        LineSeries Ls { get; }
        //in the IMPLEMENTATION : private IAnomalyGraphModel graphModel;
        ScatterSeries Normal { get; }
        ScatterSeries ANormal { get; }
        LinearAxis BottomAxis { get; }
        LinearAxis LeftAxis { get; }

        string Legend { get; } 
        bool IsFeature2Exists { get; }
        Series CorrelationObject { get; }

        
        
        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;

    }
}