using Microsoft.Maui.Graphics.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing3D;

namespace Ecet230FinalProject
{
    public class LineDrawable : BaseGraphData, IDrawable
    {
        private const int numberOfGraphs = 4; //adding the 0 line
        private string[] ColorName = new string[numberOfGraphs] { "Red", "Blue", "DarkGreen", "Black" }; //black is the 0 line
        ColorTypeConverter converter = new ColorTypeConverter();
        private int[] lineWidth = new int[numberOfGraphs] { 2, 2, 2, 1 }; //adding the 0 line


        
        public BaseGraphData[] lineGraphs = new BaseGraphData[numberOfGraphs];
        // default constructor
        public LineDrawable() : base()
        {
            for (int i = 0; i < numberOfGraphs; i++)
            {
                lineGraphs[i] = new BaseGraphData
                    (
                    Yaxis: 0,
                    Xaxis: 0,
                    lineColor: (Color)(converter.ConvertFromInvariantString(ColorName[i])),
                    lineSize: lineWidth[i],
                    newGraph: true
                    );
            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            for (int graphIndex = 0; graphIndex < lineGraphs.Length; graphIndex++)
            {
                Rect lineGraphRect = new(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
                DrawLineGraph(canvas, lineGraphRect, lineGraphs[graphIndex]);
            }
        }

        private void DrawLineGraph(ICanvas canvas, Rect lineGraphRect, BaseGraphData lineGraph)
        {
            if (lineGraph.Xaxis < 2)
            {
                lineGraph.pointArray[lineGraph.Xaxis] = lineGraph.Yaxis;
                lineGraph.Xaxis++;
                return;
            }
            else if (lineGraph.Xaxis < 1000)
            {
                lineGraph.pointArray[lineGraph.Xaxis] = lineGraph.Yaxis;
                lineGraph.Xaxis++;
            }
            else
            {
                for (int i = 0; i < 999; i++)
                {
                    lineGraph.pointArray[i] = lineGraph.pointArray[i + 1];
                }
                lineGraph.pointArray[999] = lineGraph.Yaxis;

            }
            for (int i = 0; i < lineGraph.Xaxis - 1; i++)
            {
                canvas.StrokeColor = lineGraph.lineColor;
                canvas.StrokeSize = lineGraph.lineSize;
                canvas.DrawLine(i, lineGraph.pointArray[i], i + 1, lineGraph.pointArray[i + 1]);
            }
        }
    }
}
