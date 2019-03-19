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
    class Ellipse : Figure
    {
        public int height;
        public int width;

        public Ellipse(Point _firstPoint, Point _secondPoint, int _thickness) : base(_firstPoint, _secondPoint, _thickness)
        { }

        public Ellipse() : base() { }

        public void DrawEllipse(int _height, int _width, Canvas canvas)
        {

            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse()
            {
                Width = _width,
                Height = _height,
                Stroke = Brushes.Black,
                StrokeThickness = thickness
            };

            canvas.Children.Add(circle);

            if (firstPoint.X < secondPoint.X && firstPoint.Y <= secondPoint.Y)
            {
                Canvas.SetLeft(circle, firstPoint.X);
                Canvas.SetTop(circle, firstPoint.Y);
            }
            else if (firstPoint.X <= secondPoint.X && firstPoint.Y > secondPoint.Y)
            {
                Canvas.SetLeft(circle, firstPoint.X);
                Canvas.SetBottom(circle, canvas.ActualHeight - firstPoint.Y);
            }
            else if (firstPoint.X > secondPoint.X && firstPoint.Y >= secondPoint.Y)
            {
                Canvas.SetRight(circle, canvas.ActualWidth - firstPoint.X);
                Canvas.SetBottom(circle, canvas.ActualHeight - firstPoint.Y);
            }
            else if (firstPoint.X >= secondPoint.X && firstPoint.Y < secondPoint.Y)
            {
                Canvas.SetTop(circle, firstPoint.Y);
                Canvas.SetRight(circle, canvas.ActualWidth - firstPoint.X);
            }
        }

        public override void Draw(Canvas canvas)
        {
            height = (int)Math.Abs(firstPoint.Y - secondPoint.Y);
            width = (int)Math.Abs(firstPoint.X - secondPoint.X);

            DrawEllipse(height, width, canvas);
        }
    }
}
