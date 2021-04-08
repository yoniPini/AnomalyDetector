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
        ScatterSeries Scs { get; }
        LinearAxis BottomAxis { get; }
        LinearAxis LeftAxis { get; }
        String LegendTitle { get; }

        // INotifyPropertyChanged:
        // event PropertyChangedEventHandler PropertyChanged;

    }
}