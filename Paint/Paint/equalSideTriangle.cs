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
    class equalSideTriangle : Figure
    {
        public Point thirdPoint;
        public PointCollection pointCollection = new PointCollection();

        public equalSideTriangle(Point _firstPoint, Point _secondPoint, int _thickness) : base(_firstPoint, _secondPoint, _thickness)
        { }

        public equalSideTriangle() : base() { }

        public override void Draw(Canvas canvas)
        {
            thirdPoint.X = firstPoint.X - (secondPoint.X - firstPoint.X);
            thirdPoint.Y = secondPoint.Y;

            pointCollection.Add(firstPoint);
            pointCollection.Add(secondPoint);
            pointCollection.Add(thirdPoint);

            System.Windows.Shapes.Polygon triangle = new System.Windows.Shapes.Polygon()
            {
                Points = pointCollection,
                Stroke = Brushes.Black,
                StrokeThickness = thickness
            };
            canvas.Children.Add(triangle);
        }
    }
}
