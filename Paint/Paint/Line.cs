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
    [Serializable]
    class Line : Figure
    {
        public Line(Point _firstPoint, Point _secondPoint, int _thickness) : base(_firstPoint, _secondPoint, _thickness)
        { }

        public Line() : base() { }

        public override void Draw(Canvas canvas)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
            line.X1 = firstPoint.X;
            line.Y1 = firstPoint.Y;
            line.X2 = secondPoint.X;
            line.Y2 = secondPoint.Y;
            line.Stroke = Brushes.Black;
            line.StrokeThickness = thickness;
            hash = line.GetHashCode();
            canvas.Children.Add(line);

            height = (int)Math.Abs(firstPoint.Y - secondPoint.Y);
            width = (int)Math.Abs(firstPoint.X - secondPoint.X);

        }
    }
}
