using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paint
{
    class Line : Figure
    {
        public Line(Point _firstPoint, Point _secondPoint, int _thickness) : base(_firstPoint, _secondPoint, _thickness)
        { }

        public Line() : base() { }

        public override void Draw(Canvas canvas)
        {
            canvas.Children.Add(new System.Windows.Shapes.Line()
            {
                X1 = firstPoint.X,
                Y1 = firstPoint.Y,
                X2 = secondPoint.X,
                Y2 = secondPoint.Y,
                Stroke = Brushes.Black,
                StrokeThickness = thickness
            });
        }
    }
}
