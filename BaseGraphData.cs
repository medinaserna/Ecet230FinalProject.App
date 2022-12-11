using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecet230FinalProject
{
    public class BaseGraphData
    {
        public int Yaxis { get; set; } = 0;
        public int Xaxis { get; set; } = 0;
        public int[] pointArray { get; set; }
        public Color lineColor { get; set; }

        public int lineSize { get; set; }
        public bool newGraph { get; set; } = true;

        // default constructor
        public BaseGraphData()
        {

        }
        // constructor
        public BaseGraphData(
            int Yaxis,
            int Xaxis,
            Color lineColor,
            int lineSize,
            bool newGraph)
        {
            this.Yaxis = Yaxis;
            this.Xaxis = Xaxis;
            pointArray = new int[1000];
            this.lineColor = lineColor;
            this.lineSize = lineSize;
            this.newGraph = newGraph;
        }
    }
}
