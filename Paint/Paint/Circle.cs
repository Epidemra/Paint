using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Paint
{
    class Circle : Ellipse
    {
        public Circle(Point _firstPoint, Point _secondPoint, int _thickness) : base(_firstPoint, _secondPoint, _thickness)
        { }

        public Circle() : base() { }

        public override void Draw(Canvas canvas)
        {
            height = (int)Math.Sqrt(Math.Pow(Math.Abs(firstPoint.X - secondPoint.X), 2) + Math.Pow(Math.Abs(firstPoint.Y - secondPoint.Y), 2));
            width = height;

            DrawEllipse(height, width, canvas);
        }
    }
}
