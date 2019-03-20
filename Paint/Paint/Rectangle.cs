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
    class Rectangle : Figure
    {
        public int height;
        public int width;

        public Rectangle(Point _firstPoint, Point _secondPoint, int _thickness) : base(_firstPoint, _secondPoint, _thickness)
        { }

        public Rectangle() : base() { }

        public void DrawRectangle(int _height, int _width, Canvas canvas)
        {
            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle()
            {
                Width = _width,
                Height = _height,
                Stroke = Brushes.Black,
                StrokeThickness = thickness
            };

            canvas.Children.Add(rectangle);

            if (firstPoint.X < secondPoint.X && firstPoint.Y <= secondPoint.Y)
            {
                Canvas.SetLeft(rectangle, firstPoint.X);
                Canvas.SetTop(rectangle, firstPoint.Y);
            }
            else if (firstPoint.X <= secondPoint.X && firstPoint.Y > secondPoint.Y)
            {
                Canvas.SetLeft(rectangle, firstPoint.X);
                Canvas.SetBottom(rectangle, canvas.ActualHeight - firstPoint.Y);
            }
            else if (firstPoint.X > secondPoint.X && firstPoint.Y >= secondPoint.Y)
            {
                Canvas.SetRight(rectangle, canvas.ActualWidth - firstPoint.X);
                Canvas.SetBottom(rectangle, canvas.ActualHeight - firstPoint.Y);
            }
            else if (firstPoint.X >= secondPoint.X && firstPoint.Y < secondPoint.Y)
            {
                Canvas.SetTop(rectangle, firstPoint.Y);
                Canvas.SetRight(rectangle, canvas.ActualWidth - firstPoint.X);
            }
        }


        public override void Draw(Canvas canvas)
        {
            height = (int)Math.Abs(firstPoint.Y - secondPoint.Y);
            width = (int)Math.Abs(firstPoint.X - secondPoint.X);

            DrawRectangle(height, width, canvas);
        }
    }
}
