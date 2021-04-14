using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

// interface to shapes to return landmarks of pulling a pen that make the outer line of the shape.
namespace DLL
{
    public interface IOxyPlotDrawable
    {
        List<DataPoint> GetShape();
    }
}
