using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Paint
{
    public abstract class Figure
    {
        public Point firstPoint;
        public Point secondPoint;
        public int thickness;

        public Figure(Point firstPoint, Point secondPoint, int thickness)
        {
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;
            this.thickness = thickness;
        }

        public Figure()
        {
            firstPoint = new Point(0, 0);
            secondPoint = new Point(0, 0);
            thickness = 1;
        }


        public abstract void Draw(Canvas canvas);
    }
}
